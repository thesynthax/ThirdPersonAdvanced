/**
*    Copyright (c) 2018-3018 Silvius Inc.
*/

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* About CameraController
* -> Handles camera input and camera movement
*/

public class CameraController : MonoBehaviour
{
	#region Variables and References
	//Public
	[Header("References")]
	public Transform target;
	public Transform cam;
	public StateManager stateMgr;
	[Header("Variables")]
	public float distance = 5f;

	public float yRotMax = 85f;
	public float yRotMin = -30f;

	public float sensitivity = 10f;
	public float camMoveSpeed = 5f;
	public float returnSpeed = 9f;
	public float wallPush = 0.7f;

	public float rotSmoothTime = 10f;

	public float offsetX = 0f;

	public LayerMask collision;

	//Private
	private float xRot, yRot;
	private Vector3 currRot;

	private float mouseX;
	private float mouseY;

	private Vector3 rotSmoothSpeed;

	private bool yRotLock = false;
	#endregion

	#region Methods
	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		currRot = transform.localEulerAngles;
	}

	private void Update()
	{
		cam.localPosition = new Vector3(offsetX, cam.localPosition.y, cam.localPosition.z);

		mouseX = Input.GetAxis("Mouse X");
		mouseY = Input.GetAxis("Mouse Y");
		if (!yRotLock)
		{
			xRot += mouseX * sensitivity;
			yRot -= mouseY * sensitivity;

			yRot = Mathf.Clamp(yRot, yRotMin, yRotMax);
			currRot = Vector3.Lerp(currRot, new Vector3(yRot, xRot), Time.deltaTime * rotSmoothTime);
		}
		else
		{
			xRot += mouseX * sensitivity;
			yRot = yRotMax;
			currRot = Vector3.Lerp(currRot, new Vector3(yRot, xRot), Time.deltaTime * rotSmoothTime);
		}

		transform.eulerAngles = currRot;
		Vector3 e = transform.eulerAngles;
		e.x = 0;
		target.eulerAngles = e;

		RaycastHit hit;

		if (Physics.Linecast(target.position, target.position - transform.forward * distance, out hit, collision))
		{
			Vector3 norm = hit.normal * wallPush;
			Vector3 p = hit.point + norm;

			if (Vector3.Distance(Vector3.Lerp(transform.position, p, camMoveSpeed * Time.deltaTime), target.position) <= 1f)
			{

			}
			else
			{
				transform.position = Vector3.Lerp(transform.position, p, camMoveSpeed * Time.deltaTime);
			}
			return;
		}

		yRotLock = false;

		transform.position = Vector3.Lerp(transform.position, target.position - transform.forward * distance, returnSpeed * Time.deltaTime);

		//ControlFOV(stateMgr.charStates.curState, Time.deltaTime, 1f);
	}

	private void ControlFOV(int curState, float time, float speed)
	{
		float normalFOV = 60f;
		float increasedFOV = 80f;

		if (curState == 3)
			cam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(increasedFOV, normalFOV, time * speed);
		else
			cam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(normalFOV, increasedFOV, time * speed);
	}
	#endregion


}
