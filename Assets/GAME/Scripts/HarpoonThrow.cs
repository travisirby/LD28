using UnityEngine;
using System.Collections;

public class HarpoonThrow : MonoBehaviour {

	public float harpoonForce = 200;

	[System.NonSerialized]
	public bool throwingHarpoon;

	private HarpoonDetector harpoonDetector;
	private bool isReady, isThisMyObject;

	void Awake()
	{
		if (TNManager.isThisMyObject)
		{
			isThisMyObject = true;
		}
	}

	void Start ()
	{
		Invoke("SetIsReady",2f);
	}

	void SetIsReady () { isReady = true; }

	void SetupHarpoonDetector(HarpoonDetector hD)
	{
		harpoonDetector = hD;
	}

	void Update ()
	{
		if (harpoonDetector == null) return;

		if(Input.GetMouseButtonDown(0) && isReady && isThisMyObject && harpoonDetector.holdingHarpoon)
		{
			throwingHarpoon = true;
			harpoonDetector.holdingHarpoon = false;
			Invoke ("ResetThrowingHarpoon", 3f);

			Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3 clickDistance = mouseWorldPos - transform.position;

			transform.parent = null;

			Vector3 mousePos = Input.mousePosition;
			mousePos.z = 10f; //The distance from the camera to the player object
			Vector3 lookPos = Camera.main.ScreenToWorldPoint(mousePos);
			lookPos = lookPos - transform.position;
			float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

			rigidbody2D.AddForce( clickDistance.normalized * harpoonForce);

		}
	}

	void ResetThrowingHarpoon () { throwingHarpoon = false; }

//	void OnTriggerEnter2D (Collider2D col)
//	{
//
//		if (col.CompareTag("Player"))
//		{
//			if (isThisMyObject && !holdingHarpoon && !throwingHarpoon)
//			{
//				Debug.Log("mine");
//				holdingHarpoon = true;
//				transform.parent = col.transform;
//				transform.localPosition = Vector3.zero;
//				transform.rotation = Quaternion.identity;
//			}
//			else if (!isThisMyObject)
//			{
//				Debug.Log("hit");
//			}
//		}
//	}
}
