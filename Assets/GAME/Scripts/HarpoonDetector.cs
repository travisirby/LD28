﻿using UnityEngine;
using System.Collections;

public class HarpoonDetector : MonoBehaviour {

	public int harpoonOwnedLayer = 12;

	[System.NonSerialized]
	public int ownerID;

	private bool isThisMyObject, isReady, holdingHarpoon, throwingHarpoon;
	private Transform harpoon;

	void Awake()
	{
		if (TNManager.isThisMyObject)
		{
			isThisMyObject = true;
		}
		ownerID = TNManager.objectOwnerID;
		Invoke("SetIsReady", 2f);
	}

	void SetIsReady () { isReady = true; }
	

	void ThrewHarpoon ()			// Sender is HarpoonThrower
	{
		holdingHarpoon = false;
		throwingHarpoon = true;
		Invoke ("ResetThrowingHarpoon", 3f);
	}

	void ResetThrowingHarpoon () { throwingHarpoon = false; }

	void OnTriggerEnter2D (Collider2D col)
	{
		if (!isThisMyObject || !isReady) return;  // Is THIS player our object?? Does not mean the harpoon we are colliding with 

		if (col.CompareTag("Harpoon"))
		{
			if (!holdingHarpoon && !throwingHarpoon && col.gameObject.layer != harpoonOwnedLayer)
			{
				if (col.rigidbody2D.velocity.magnitude > 0.1f)
				{
					return;
				}
				holdingHarpoon = true;
				harpoon = col.transform;
				harpoon.SendMessage ("SetOwner", TNManager.playerID);
			}
		}
	}

	void OnTriggerStay2D (Collider2D col)
	{
		if (!isThisMyObject || !isReady) return;
		
		if (col.CompareTag("Harpoon"))
		{
			if (!holdingHarpoon && !throwingHarpoon && col.gameObject.layer != harpoonOwnedLayer)
			{
				if (col.rigidbody2D.velocity.magnitude > 0.1f)
				{
					return;
				}
				holdingHarpoon = true;
				harpoon = col.transform;
				harpoon.SendMessage ("SetOwner", TNManager.playerID);
			}
		}
	}


}
