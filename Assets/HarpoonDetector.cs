using UnityEngine;
using System.Collections;

public class HarpoonDetector : MonoBehaviour {

	[System.NonSerialized]
	public bool holdingHarpoon;

	private bool isReady, isThisMyObject, throwingHarpoon;
	private HarpoonThrow harpoonThrow;

	void Awake()
	{
		if (TNManager.isThisMyObject)
		{
			isThisMyObject = true;
		}
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		Debug.Log (col.tag);
		if (col.CompareTag("Harpoon"))
		{
			if (harpoonThrow == null && isThisMyObject) 
			{
				Debug.Log("triggER");
				harpoonThrow = col.gameObject.GetComponent<HarpoonThrow>();
				col.SendMessage("SetupHarpoonDetector", this, SendMessageOptions.DontRequireReceiver);
			}
			else if (isThisMyObject && !holdingHarpoon && !harpoonThrow.throwingHarpoon)
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

	void OnTriggerStay2D (Collider2D col)
	{
		if (col.CompareTag("Harpoon"))
		{
			if (harpoonThrow == null && isThisMyObject) 
			{
				harpoonThrow = col.gameObject.GetComponent<HarpoonThrow>();
				col.SendMessage("SetupHarpoonDetector", this, SendMessageOptions.DontRequireReceiver);
			}
			else if (isThisMyObject && !holdingHarpoon && !harpoonThrow.throwingHarpoon)
			{
				Debug.Log("triggER");
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
