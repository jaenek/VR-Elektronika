﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField]
	private Transform target;
	[SerializeField]
	private Vector3 offset;
	[SerializeField]
	private float mouseSensitivity;
	private float xAxisClamp; 

	void Update()
	{
		Cursor.lockState = CursorLockMode.Locked;
		RotateCamera();

		if (target == null)
			return;

		transform.position = target.position + offset;
	}

	void RotateCamera()
	{
		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = Input.GetAxis("Mouse Y");

		float rotX = mouseX * mouseSensitivity;
		float rotY = mouseY * mouseSensitivity;

		xAxisClamp -= rotY;

		Vector3 rotCamera = transform.rotation.eulerAngles;
		Vector3 rotTarget = target.transform.rotation.eulerAngles;

		rotCamera.x -= rotY;
		rotCamera.z = 0;
		rotCamera.y += rotX;
		rotTarget.y += rotX;

		if (xAxisClamp > 90)
		{
			xAxisClamp = 90;
			rotCamera.x = 90;
		}
		else if (xAxisClamp < -90)
		{
			xAxisClamp = -90;
			rotCamera.x = 270;
		}

		transform.rotation = Quaternion.Euler(rotCamera);
		target.transform.rotation = Quaternion.Euler(rotTarget);
	}
}