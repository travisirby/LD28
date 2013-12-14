using UnityEngine;
using System.Collections;

public class HarpoonDetector : MonoBehaviour {

	private bool isThisMyObject, holdingHarpoon, throwingHarpoon;

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
			if (isThisMyObject && !holdingHarpoon && !throwingHarpoon)
			{
				Debug.Log("mine");
				holdingHarpoon = true;
				col.transform.parent = transform;
				col.transform.localPosition = Vector3.zero;
				col.transform.rotation = Quaternion.identity;
			}
			else if (!isThisMyObject)
			{
				Debug.Log("hit");
			}
		}
	}
}
