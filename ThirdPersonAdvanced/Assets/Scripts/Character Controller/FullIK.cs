/**
*    Copyright (c) 2018-3018 Silvius Inc.
*/

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* About FootIK
* ->
*/

public class FullIK : MonoBehaviour 
{
	#region Variables and References
	private Animator anim;

	public StateManager stateMgr;
	[Header("Look IK")]
	public float lookIKWeight;
	public float bodyWeight;
	public float headWeight;
	public float eyesWeight;
	public float clampValue;

	public Transform lookAt;

	[Header("Feet IK")]
	[Range(0f, 2f)] [SerializeField] private float heightFromGroundRaycast = 1.14f;
	[Range(0f, 2f)] [SerializeField] private float raycastDownDist = 1.5f;
	[SerializeField] private float pelvisOffset = 0f;
	[Range(0f, 1f)] [SerializeField] private float pelvisMoveSpeed = 0.28f;
	[Range(0f, 1f)] [SerializeField] private float feetToIKPosSpeed = 0.5f;

	public bool FeetIK = true;
	public bool useProIK = false;
	public bool debugSolver = true;

	private Vector3 lFPos, rFPos, lFIKPos, rFIKPos;
	private Quaternion lFIKRot, rFIKRot;
	private float lastPelvisPosY, lastRFPos, lastLFPos;

	private LayerMask environment;
	#endregion

	#region Methods
	private void Start()
	{
		anim = stateMgr.anim;
		environment = stateMgr.ground;
	}

	private void OnAnimatorIK(int layerIndex)
	{
		anim.SetLookAtWeight(lookIKWeight, bodyWeight, headWeight, eyesWeight, clampValue);
		anim.SetLookAtPosition(lookAt.position);

		if (FeetIK)
		{
			MovePelvisHeight();

			anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
			if (useProIK)
				anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, anim.GetFloat(AnimVars.RightFoot));
			MoveFeetToIK(AvatarIKGoal.RightFoot, rFIKPos, rFIKRot, ref lastRFPos);

			anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
			if (useProIK)
				anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, anim.GetFloat(AnimVars.LeftFoot));
			MoveFeetToIK(AvatarIKGoal.LeftFoot, lFIKPos, lFIKRot, ref lastLFPos);
		}
	}

	private void FixedUpdate()
	{
		Ray ray = new Ray(stateMgr.mainCam.position, stateMgr.mainCam.forward);
		lookAt.position = ray.GetPoint(15f);
		lookAt.position = new Vector3(lookAt.position.x, 1.75f, lookAt.position.z);
		if (FeetIK)
		{
			AdjustFeetTarget(ref rFPos, HumanBodyBones.RightFoot);
			AdjustFeetTarget(ref lFPos, HumanBodyBones.LeftFoot);

			FeetPosSolver(rFPos, ref rFIKPos, ref rFIKRot);
			FeetPosSolver(rFPos, ref lFIKPos, ref lFIKRot);
		}
	}

	private void MoveFeetToIK(AvatarIKGoal foot, Vector3 IKpos, Quaternion IKrot, ref float lastFootPosY)
	{
		Vector3 targetIkPos = anim.GetIKPosition(foot);

		if (IKpos != Vector3.zero)
		{
			targetIkPos = transform.InverseTransformPoint(targetIkPos);
			IKpos = transform.InverseTransformPoint(IKpos);

			float yVariable = Mathf.Lerp(lastFootPosY, IKpos.y, feetToIKPosSpeed);
			targetIkPos.y += yVariable;

			lastFootPosY = yVariable;

			targetIkPos = transform.TransformPoint(targetIkPos);

			anim.SetIKRotation(foot, IKrot);
		}

		anim.SetIKPosition(foot, targetIkPos);
	}

	private void MovePelvisHeight()
	{
		if (rFIKPos == Vector3.zero || lFIKPos == Vector3.zero || lastPelvisPosY == 0)
		{
			lastPelvisPosY = anim.bodyPosition.y;
			return;
		}

		float lOffsetPosition = lFIKPos.y - transform.position.y;
		float rOffsetPosition = rFIKPos.y - transform.position.y;

		float totalOffset = (lOffsetPosition < rOffsetPosition) ? lOffsetPosition : rOffsetPosition;

		Vector3 newPelvisPosition = anim.bodyPosition + Vector3.up * totalOffset;

		newPelvisPosition.y = Mathf.Lerp(lastPelvisPosY, newPelvisPosition.y, pelvisMoveSpeed);

		anim.bodyPosition = newPelvisPosition;

		lastPelvisPosY = anim.bodyPosition.y;
	}

	private void FeetPosSolver(Vector3 fromSky, ref Vector3 feetIKPos, ref Quaternion feetIKRot)
	{
		RaycastHit feetOutHit;

		if (debugSolver)
			Debug.DrawLine(fromSky, fromSky + Vector3.down * (raycastDownDist + heightFromGroundRaycast), Color.yellow);

		if (Physics.Raycast(fromSky, Vector3.down, out feetOutHit, raycastDownDist + heightFromGroundRaycast, environment))
		{
			feetIKPos = fromSky;
			feetIKPos.y = feetOutHit.point.y + pelvisOffset;
			feetIKRot = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;

			return;
		}

		feetIKPos = Vector3.zero;
	}

	private void AdjustFeetTarget(ref Vector3 feetPos, HumanBodyBones foot)
	{
		feetPos = anim.GetBoneTransform(foot).position;
		feetPos.y = transform.position.y + heightFromGroundRaycast;
	}
	#endregion
}
