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
	public Hashes hashes;
	public UserInput uInput;
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
	public LayerMask ground;
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
		UpdateStates();
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

	private void UpdateStates()
	{
		if (fwd == 0)
		{
			charStates.curState = 1;
		}
		else if (fwd > 0 && !uInput.sprint)
		{
			charStates.curState = 2;
		}
		else if (fwd > 0 && uInput.sprint)
		{
			charStates.curState = 3;
		}
	}

	private bool RequirementsCleared()
	{
		if (anim.isHuman)
			return true;
		return false;
	}
	#endregion
}
