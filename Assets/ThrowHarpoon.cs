using UnityEngine;
using System.Collections;

public class ThrowHarpoon : MonoBehaviour {

	public float harpoonForce = 200;

	private bool isReady, isThisMyObject, holdingHarpoon = true, throwingHarpoon;

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
	
	void Update ()
	{
		if(Input.GetMouseButtonDown(0) && isReady && isThisMyObject && holdingHarpoon)
		{
			throwingHarpoon = true;
			holdingHarpoon = false;
			Invoke ("ResetThrowingHarpoon", 3f);

			Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3 clickDistance = mouseWorldPos - transform.position;

			transform.parent = null;
//
//			Vector3 upAxis = new Vector3(0,0,1);
//			Vector3 mouseScreenPosition = Input.mousePosition;
//			//set mouses z to your targets
//			mouseScreenPosition.z = transform.position.z;
//			Vector3 mouseWorldSpace = Camera.mainCamera.ScreenToWorldPoint(mouseScreenPosition);
//			transform.LookAt(mouseWorldSpace, upAxis);
//			//zero out all rotations except the axis I want
//			transform.eulerAngles = new Vector3(0,0,transform.eulerAngles.z);

			Vector3 mousePos = Input.mousePosition;
			mousePos.z = 10f; //The distance from the camera to the player object
			Vector3 lookPos = Camera.main.ScreenToWorldPoint(mousePos);
			lookPos = lookPos - transform.position;
			float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

			//var angle = Mathf.Atan2(Input.mousePosition.x, Input.mousePosition.y) * Mathf.Rad2Deg;

			//transform.localEulerAngles = new Vector3 (0f, 0f, angle);

			rigidbody2D.AddForce( clickDistance.normalized * harpoonForce);

		}
	}

	void ResetThrowingHarpoon () { throwingHarpoon = false; }

	void OnTriggerEnter2D (Collider2D col)
	{

		if (col.CompareTag("Player"))
		{
			if (isThisMyObject && !holdingHarpoon && !throwingHarpoon)
			{
				Debug.Log("mine");
				holdingHarpoon = true;
				transform.parent = col.transform;
				transform.localPosition = Vector3.zero;
				transform.rotation = Quaternion.identity;
			}
			else if (!isThisMyObject)
			{
				Debug.Log("hit");
			}
		}
	}
}
