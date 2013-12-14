using UnityEngine;
using System.Collections;

public class HarpoonThrow : MonoBehaviour {

	public float harpoonForce = 200;

	[System.NonSerialized]
	public bool throwingHarpoon;

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

	void Update ()
	{
		if (transform.parent == null) return;

		if(Input.GetMouseButtonDown(0) && isReady && isThisMyObject && !throwingHarpoon)
		{
			throwingHarpoon = true;
			transform.parent.SendMessage ("ThrewHarpoon", SendMessageOptions.DontRequireReceiver);
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

	void OnCollisionEnter2D (Collision2D col)
	{
		rigidbody2D.velocity = Vector2.zero;
	}
}
