/**
*    Copyright (c) 2018-3018 Silvius Inc.
*/

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* About StateManager
* -> Contains all the variables for the character controller
*/

public class StateManager : MonoBehaviour 
{
	#region Variables and References
	//Public
	[Header("References")]
	public Transform modelPlaceholder;
	public GameObject modelInit;

	public Animator anim;
	public CapsuleCollider coll;
	public Rigidbody rBody;

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

	}

	private bool RequirementsCleared()
	{
		return true;
	}
	#endregion
}
