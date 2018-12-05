/**
*    Copyright (c) 2018-3018 Silvius Inc.
*/

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* About PlayerMovement
* -> Converts code into game mechanics
*/

public class PlayerMovement : MonoBehaviour 
{
	#region Variables and References
	private StateManager states;
	private UserInput uInput;
	#endregion

	#region Methods
	public void Init(UserInput ui, StateManager st)
	{
		states = st;
		uInput = ui;
	}

	public void Tick()
	{
		states.charStates.onGround = OnGround();
	}

	private bool OnGround()
	{
		bool r = false;

		Vector3 origin = transform.position + (Vector3.up * 0.55f);

		RaycastHit hit = new RaycastHit();
		bool isHit = false;
		FindGround(origin, ref hit, ref isHit);

		if (!isHit)
		{
			for (int i = 0; i < 4; i++)
			{
				Vector3 newOrigin = origin;

				switch (i)
				{
					case 0:
						newOrigin += Vector3.forward / 3;
						break;
					case 1:
						newOrigin -= Vector3.forward / 3;
						break;
					case 2:
						newOrigin += Vector3.right / 3;
						break;
					case 3:
						newOrigin -= Vector3.right / 3;
						break;
				}

				FindGround(newOrigin, ref hit, ref isHit);

				if (isHit)
				{
					break;
				}
			}
		}

		r = isHit;

		if (r != false)
		{
			Vector3 targetPosition = transform.position;
			targetPosition.y = hit.point.y;
			transform.position = targetPosition;
		}

		return r;
	}

	private void FindGround(Vector3 origin, ref RaycastHit hit, ref bool isHit)
	{
		Debug.DrawRay(origin, -Vector3.up * 0.5f, Color.red);
		if (Physics.Raycast(origin, -Vector3.up, out hit, states.groundDistance))
		{
			isHit = true;
		}
	}
	#endregion
}
