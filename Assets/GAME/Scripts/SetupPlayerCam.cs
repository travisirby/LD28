using UnityEngine;
using System.Collections;

public class SetupPlayerCam : MonoBehaviour {
	
	bool isThisMyObject;

	void Awake () 
	{
		isThisMyObject = TNManager.isThisMyObject;
	}
	
	void Start () 
	{
		if (isThisMyObject)
		{
			Camera.main.SendMessage("SetupPlayerCam", transform);
			Camera.main.SendMessage("SetupPlayerRigidbody", rigidbody2D);
		}
	}
}
