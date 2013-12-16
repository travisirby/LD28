using UnityEngine;
using System.Collections;

public class SetupPlayerCam : MonoBehaviour {

	private bool isThisMyObject;

	void Awake () 
	{
		isThisMyObject = TNManager.isThisMyObject;
	}
	
	void Start () 
	{
		if (isThisMyObject)
		{
			Camera.main.SendMessage("SetupPlayerCam", transform);
		}
	}
}
