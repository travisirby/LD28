﻿using UnityEngine;
using System.Collections;

public class HarpoonDetector : MonoBehaviour {

	public int harpoonOwnedLayer = 12;

	[System.NonSerialized]
	public int ownerID;

	private bool isThisMyObject, holdingHarpoon, throwingHarpoon;
	private Transform harpoon;

	void Awake()
	{
		if (TNManager.isThisMyObject)
		{
			isThisMyObject = true;
		}
		ownerID = TNManager.objectOwnerID;
	}
	

	void ThrewHarpoon ()			// Sender is HarpoonThrower
	{
		holdingHarpoon = false;
		throwingHarpoon = true;
		Invoke ("ResetThrowingHarpoon", 3f);
	}

	void ResetThrowingHarpoon () { throwingHarpoon = false; }

	void OnTriggerEnter2D (Collider2D col)
	{
		if (!isThisMyObject) return;

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
		if (!isThisMyObject) return;
		
		if (col.CompareTag("Harpoon"))
		{
			if (!holdingHarpoon && !throwingHarpoon && col.gameObject.layer != harpoonOwnedLayer)
			{
				if (col.rigidbody2D.velocity.magnitude > 0.5f)
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