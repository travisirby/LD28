using UnityEngine;
using System.Collections;

public class Rigidbody3DFollow : MonoBehaviour {

	public Transform target;

	private bool isThisMyObject;

	void Awake()
	{
		isThisMyObject = TNManager.isThisMyObject;
	}

	void FixedUpdate()
	{
		if (isThisMyObject)
		{
			rigidbody.MovePosition(target.position);
			rigidbody.MoveRotation(target.rotation);
		}
	}
}
