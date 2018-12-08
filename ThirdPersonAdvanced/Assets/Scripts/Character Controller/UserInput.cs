/**
*    Copyright (c) 2018-3018 Silvius Inc.
*/

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* About UserInput
* -> Handles user input and changes variables accordingly
*/

[RequireComponent(typeof(StateManager))]
public class UserInput : MonoBehaviour 
{
	#region Variables and References
	public StateManager stateMgr;
	public PlayerMovement pMove;

	private float horizontal;
	private float vertical;
	private bool jump;
	private bool walk;
	private bool sprint;
	#endregion

	#region Methods
	private void Start()
	{
		stateMgr.Init();
		pMove.Init(this, stateMgr);

		if (Camera.main)
			stateMgr.mainCam = Camera.main.transform;
	}

	private void Update()
	{
		UpdateInputs(stateMgr.mainCam, ref stateMgr.moveDir);

		stateMgr.Tick();
		pMove.Tick();


	}

	private void OnAnimatorMove()
	{
		pMove.OnAnimMove(stateMgr.charStates.onGround, Time.deltaTime, stateMgr.anim, stateMgr.rBody);
	}

	private void UpdateInputs(Transform cam, ref Vector3 moveDir)
	{
		horizontal = Input.GetAxis("Horizontal");
		vertical = Input.GetAxis("Vertical");
		jump = Input.GetButtonDown("Jump");
		sprint = Input.GetKey(KeyCode.LeftShift);
		walk = Input.GetKey(KeyCode.LeftControl);

		if (cam)
		{
			Vector3 camF = Vector3.Scale(cam.forward, new Vector3(1, 0, 1).normalized);
			moveDir = horizontal * cam.right + vertical * camF;
		}
		else
		{
			moveDir = horizontal * Vector3.right + vertical * Vector3.forward;
		}
	}
	#endregion
}
