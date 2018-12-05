/**
*    Copyright (c) 2018-3018 Silvius Inc.
*/

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* About UserInput
* -> Handles user input and changes variables accordingly
*/

public class UserInput : MonoBehaviour 
{
	#region Variables and References
	public StateManager stateMgr;
	public PlayerMovement pMove;
	#endregion

	#region Methods
	private void Start()
	{
		stateMgr.Init();
		pMove.Init(this, stateMgr);
	}

	private void Update()
	{
		stateMgr.Tick();
		pMove.Tick();
	}
	#endregion
}
