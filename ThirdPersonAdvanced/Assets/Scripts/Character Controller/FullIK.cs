﻿/**
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
	public bool LookIK = true;
	public Transform lookAt;

	[Header("Feet IK")]
	public bool enableFeetIk = true;
    [Range(0, 2)] [SerializeField] private float heightFromGroundRaycast = 1.14f;
    [Range(0, 2)] [SerializeField] private float raycastDownDistance = 1.5f;
    [SerializeField] private LayerMask environmentLayer;
    [SerializeField] private float pelvisOffset = 0f;
    [Range(0, 1)] [SerializeField] private float pelvisUpAndDownSpeed = 0.28f;
    [Range(0, 1)] [SerializeField] private float feetToIkPositionSpeed = 0.5f;

    public string leftFootAnimVariableName = "LeftFootCurve";
    public string rightFootAnimVariableName = "RightFootCurve";

    public bool useProIkFeature = false;
    public bool showSolverDebug = true;

	private Vector3 rightFootPosition, leftFootPosition, leftFootIkPosition, rightFootIkPosition;
    private Quaternion leftFootIkRotation, rightFootIkRotation;
    private float lastPelvisPositionY, lastRightFootPositionY, lastLeftFootPositionY;

	private LayerMask environment;
	#endregion

	#region Methods
	private void Start()
	{
		anim = stateMgr.anim;
		environment = stateMgr.ground;
	}

	private void FixedUpdate()
	{
		Ray ray = new Ray(stateMgr.mainCam.position, stateMgr.mainCam.forward);
		lookAt.position = ray.GetPoint(15f);
		lookAt.position = new Vector3(lookAt.position.x, 1.75f, lookAt.position.z);

		if (enableFeetIk == false) { return; }
		if (anim == null) { return; }
		if (!stateMgr.charStates.onGround) { return; }

		AdjustFeetTarget(ref rightFootPosition, HumanBodyBones.RightFoot);
		AdjustFeetTarget(ref leftFootPosition, HumanBodyBones.LeftFoot);

		//find and raycast to the ground to find positions
		FeetPositionSolver(rightFootPosition, ref rightFootIkPosition, ref rightFootIkRotation); // handle the solver for right foot
		FeetPositionSolver(leftFootPosition, ref leftFootIkPosition, ref leftFootIkRotation); //handle the solver for the left foot

	}

	private void Update()
	{
		if (stateMgr.charStates.curState == 1)
		{
			anim.SetFloat(AnimVars.RightFoot, 0.6f);
			anim.SetFloat(AnimVars.LeftFoot, 0.6f);
		}
		else
		{
			anim.SetFloat(AnimVars.RightFoot, 0);
			anim.SetFloat(AnimVars.LeftFoot, 0);
		}
	}

	private void OnAnimatorIK(int layerIndex)
	{
		if (LookIK)
		{
			anim.SetLookAtWeight(lookIKWeight, bodyWeight, headWeight, eyesWeight, clampValue);
			anim.SetLookAtPosition(lookAt.position);
		}
		if (enableFeetIk == false) { return; }
		if (anim == null) { return; }
		if (!stateMgr.charStates.onGround) { return; }

		MovePelvisHeight();

		//right foot ik position and rotation -- utilise the pro features in here
		anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

		if (useProIkFeature)
		{
			anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, anim.GetFloat(rightFootAnimVariableName));
		}

		MoveFeetToIkPoint(AvatarIKGoal.RightFoot, rightFootIkPosition, rightFootIkRotation, ref lastRightFootPositionY);

		//left foot ik position and rotation -- utilise the pro features in here
		anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);

		if (useProIkFeature)
		{
			anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, anim.GetFloat(leftFootAnimVariableName));
		}

		MoveFeetToIkPoint(AvatarIKGoal.LeftFoot, leftFootIkPosition, leftFootIkRotation, ref lastLeftFootPositionY);
	}

	void MoveFeetToIkPoint(AvatarIKGoal foot, Vector3 positionIkHolder, Quaternion rotationIkHolder, ref float lastFootPositionY)
	{
		Vector3 targetIkPosition = anim.GetIKPosition(foot);

		if (positionIkHolder != Vector3.zero)
		{
			targetIkPosition = transform.InverseTransformPoint(targetIkPosition);
			positionIkHolder = transform.InverseTransformPoint(positionIkHolder);

			float yVariable = Mathf.Lerp(lastFootPositionY, positionIkHolder.y, feetToIkPositionSpeed);
			targetIkPosition.y += yVariable;

			lastFootPositionY = yVariable;

			targetIkPosition = transform.TransformPoint(targetIkPosition);

			anim.SetIKRotation(foot, rotationIkHolder);
		}

		anim.SetIKPosition(foot, targetIkPosition);
	}

	private void MovePelvisHeight()
	{

		if (rightFootIkPosition == Vector3.zero || leftFootIkPosition == Vector3.zero || lastPelvisPositionY == 0)
		{
			lastPelvisPositionY = anim.bodyPosition.y;
			return;
		}

		float lOffsetPosition = leftFootIkPosition.y - transform.position.y;
		float rOffsetPosition = rightFootIkPosition.y - transform.position.y;

		float totalOffset = (lOffsetPosition < rOffsetPosition) ? lOffsetPosition : rOffsetPosition;

		Vector3 newPelvisPosition = anim.bodyPosition + Vector3.up * totalOffset;

		newPelvisPosition.y = Mathf.Lerp(lastPelvisPositionY, newPelvisPosition.y, pelvisUpAndDownSpeed);

		anim.bodyPosition = newPelvisPosition;

		lastPelvisPositionY = anim.bodyPosition.y;

	}

	private void FeetPositionSolver(Vector3 fromSkyPosition, ref Vector3 feetIkPositions, ref Quaternion feetIkRotations)
	{
		//raycast handling section 
		RaycastHit feetOutHit;

		if (showSolverDebug)
			Debug.DrawLine(fromSkyPosition, fromSkyPosition + Vector3.down * (raycastDownDistance + heightFromGroundRaycast), Color.yellow);

		if (Physics.Raycast(fromSkyPosition, Vector3.down, out feetOutHit, raycastDownDistance + heightFromGroundRaycast, environmentLayer))
		{
			//finding our feet ik positions from the sky position
			feetIkPositions = fromSkyPosition;
			feetIkPositions.y = feetOutHit.point.y + pelvisOffset;
			feetIkRotations = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;

			return;
		}

		feetIkPositions = Vector3.zero; //it didn't work :(

	}

	private void AdjustFeetTarget(ref Vector3 feetPositions, HumanBodyBones foot)
	{
		feetPositions = anim.GetBoneTransform(foot).position;
		feetPositions.y = transform.position.y + heightFromGroundRaycast;
	}
	#endregion
}
