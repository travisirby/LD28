using UnityEngine;
using System.Collections;

public class HarpoonDetector : MonoBehaviour {

	public int harpoonOwnedLayer = 12;

	private bool isThisMyObject, holdingHarpoon, throwingHarpoon;
	private Transform harpoon;

	void Awake()
	{
		if (TNManager.isThisMyObject)
		{
			isThisMyObject = true;
		}
	}
	

	void ThrewHarpoon ()
	{
		holdingHarpoon = false;
		throwingHarpoon = true;
		Invoke ("ResetThrowingHarpoon", 3f);
	}

	void ResetThrowingHarpoon () { throwingHarpoon = false; }

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.CompareTag("Harpoon"))
		{
			if (!holdingHarpoon && !throwingHarpoon && col.gameObject.layer != harpoonOwnedLayer)
			{
				holdingHarpoon = true;

				harpoon = col.transform;
				harpoon.SendMessage ("SetOwner", TNManager.playerID);
				col.gameObject.layer = harpoonOwnedLayer; 
				col.transform.parent.transform.parent = transform;
				col.transform.position = transform.position;
				col.transform.parent.rotation = Quaternion.identity;
			}
//			else if (!isThisMyObject)
//			{
//			}
		}
	}

}
