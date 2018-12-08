/**
*    Copyright (c) 2018-3018 Silvius Inc.
*/

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* About PlayerMovement
* -> Converts variables into game mechanics
*/

public class PlayerMovement : MonoBehaviour 
{
	#region Variables and References
	private StateManager stateMgr;
	private UserInput uInput;
	#endregion

	#region Methods
	public void Init(UserInput ui, StateManager st)
	{
		stateMgr = st;
		uInput = ui;
	}

	public void Tick()
	{
		stateMgr.charStates.onGround = OnGround();
		Move(ref stateMgr.moveDir, ref stateMgr.turn, ref stateMgr.fwd);
		Animate(stateMgr.anim, stateMgr.turn, stateMgr.fwd, stateMgr.charStates.onGround);
	}

	private void Move(ref Vector3 moveDir, ref float turn, ref float fwd)
	{
		if (moveDir.magnitude > 1f) moveDir.Normalize();
		moveDir = transform.InverseTransformDirection(moveDir);
		turn = Mathf.Atan2(moveDir.x, moveDir.z);
		fwd = moveDir.z;

		ExtraTurn(stateMgr.idleTurnSpeed, stateMgr.moveTurnSpeed, stateMgr.fwd, stateMgr.turn, Time.deltaTime, 0.9f);
	}

	private void Animate(Animator anim, float turn, float fwd, bool onGround)
	{
		anim.SetFloat(AnimVars.Forward, fwd, 0.1f, Time.deltaTime);
		anim.SetFloat(AnimVars.Turn, turn, 0.1f, Time.deltaTime);
		anim.SetBool(AnimVars.OnGround, onGround);
	}

	private void ExtraTurn(float idleTurnSpeed, float moveTurnSpeed, float fwd, float turn, float time, float speed)
	{
		float turnSpeed = Mathf.Lerp(idleTurnSpeed, moveTurnSpeed, fwd) * speed;
		transform.Rotate(0, turn * turnSpeed * time, 0);
	}

	public void OnAnimMove(bool onGround, float time, Animator anim, Rigidbody rBody)
	{
		if (onGround && time > 0f)
		{
			Vector3 v = anim.deltaPosition / time;

			v.y = rBody.velocity.y;
			rBody.velocity = v;
		}
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
		if (Physics.Raycast(origin, -Vector3.up, out hit, stateMgr.groundDistance))
		{
			isHit = true;
		}
	}
	#endregion
}
