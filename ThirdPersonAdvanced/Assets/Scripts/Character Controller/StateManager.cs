/**
*    Copyright (c) 2018-3018 Silvius Inc.
*/

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* About StateManager
* -> Contains all the variables for the character controller
*/

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class StateManager : MonoBehaviour 
{
	#region Variables and References
	//Public
	[Header("References")]
	public Transform modelPlaceholder;
	public GameObject modelInit;
	[Space]
	public Animator anim;
	public CapsuleCollider coll;
	public Rigidbody rBody;
	public CharacterStates charStates;
	[Space]
	[HideInInspector] public Transform mainCam;
	[Header("Variables")]
	public Vector3 moveDir;
	public float fwd;
	public float turn;
	[Space]
	[SerializeField] private int _curState;
	[SerializeField] private bool _onGround;

	[Header("Constants")]
	public float moveTurnSpeed = 360f;
	public float idleTurnSpeed = 180f;
	public float jumpForce = 8f;
	public float groundDistance = 0.634f;
	//Private
	private GameObject activeModel;
	#endregion

	#region Methods
	public void Init()
	{
		InitModel();
		SetupComponents();
	}

	public void Tick()
	{
		UpdateCharStateNames();
	}

	private void InitModel()
	{
		activeModel = Instantiate(modelInit) as GameObject;
		activeModel.transform.parent = modelPlaceholder;
		activeModel.transform.localEulerAngles = Vector3.zero;
		activeModel.transform.localPosition = Vector3.zero;
		activeModel.transform.localScale = Vector3.one;
	}

	private void SetupComponents()
	{
		Animator modelAnim = activeModel.GetComponent<Animator>();
		anim.avatar = modelAnim.avatar;
		anim.applyRootMotion = true;
		anim.updateMode = AnimatorUpdateMode.AnimatePhysics;
		anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		Destroy(modelAnim);

		rBody.useGravity = true;
		rBody.isKinematic = false;
		rBody.constraints = RigidbodyConstraints.FreezeRotation;
	}

	private void UpdateCharStateNames()
	{
		_curState = charStates.curState;
		_onGround = charStates.onGround;
	}

	private bool RequirementsCleared()
	{
		return true;
	}
	#endregion
}
