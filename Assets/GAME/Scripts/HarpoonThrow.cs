using UnityEngine;
using System.Collections;
using TNet;

public class HarpoonThrow : TNBehaviour {

	public SmoothFollow spriteFollow;

	public float harpoonForce = 200;
	public int harpoonFreeLayer = 11;
	public int harpoonOwnedLayer = 12;

	private bool isReady, isThisMyObject, throwingHarpoon;
	private TNSyncRigidbody2D tnSync;
	private int ownerID;

	void Awake()
	{
		if (TNManager.isThisMyObject)
		{
	//		tnSync = GetComponent<TNSyncRigidbody2D>();
	//		tnSync.enabled = false;
			isThisMyObject = true;
		// 	InvokeRepeating("SyncHarpoon",2f,0.5f);
		}
	}

	void Start ()
	{
		Invoke("SetIsReady",2f);
	}

	void SetIsReady () { isReady = true; }

	void SetOwner(int id) 
	{
		ownerID = id;
	}

	void Update ()
	{
		if (transform.parent.transform.parent == null || gameObject.layer == harpoonFreeLayer)
		{
			return;
		}

		if (TNManager.playerID != ownerID)
		{
			Debug.Log ("Not Owner");
			return;
		}

		if(Input.GetMouseButtonDown(0) && isReady && !throwingHarpoon)
		{
			throwingHarpoon = true;
			Invoke ("ResetThrowingHarpoon", 3f);

			spriteFollow.enabled = true;

			transform.parent.parent.SendMessage ("ThrewHarpoon", SendMessageOptions.DontRequireReceiver);
			transform.parent.transform.parent = null;
			gameObject.layer = harpoonFreeLayer;

			Vector3 mousePos = Input.mousePosition;
			mousePos.z = 10f; //The distance from the camera to the player object
			Vector3 lookPos = Camera.main.ScreenToWorldPoint(mousePos);
			lookPos = lookPos - transform.position;
			float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

			Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3 clickDistance = mouseWorldPos - transform.position;
			rigidbody2D.AddForce( clickDistance.normalized * harpoonForce);

		}
	}

	void ResetThrowingHarpoon () { throwingHarpoon = false; }

	void OnCollisionEnter2D (Collision2D col)
	{
		rigidbody2D.velocity = Vector2.zero;
	}

//	void SyncHarpoon()
//	{
//		if (gameObject.layer == harpoonFreeLayer)
//		{
//			if (!tnSync.enabled) tnSync.enabled = true;
//		}
//		else if (TNManager.isInChannel)
//		{
//			if (tnSync.enabled = true) tnSync.enabled = false;
//			//tno.Send(100, Target.OthersSaved, transform.position, transform.eulerAngles);
//		}
//	}
	
	[RFC(100)]
	void OnSync (Vector3 pos, Vector3 rot)
	{
		Debug.Log(gameObject.GetInstanceID());
		transform.position = pos;
		transform.rotation = Quaternion.Euler(rot);
	}
}
