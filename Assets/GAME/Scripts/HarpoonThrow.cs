using UnityEngine;
using System.Collections;
using TNet;

public class HarpoonThrow : TNBehaviour {

	public TrailColliderGen harpoonTrail;
	public float harpoonForce = 200;
	public int harpoonFreeLayer = 11;
	public int harpoonOwnedLayer = 12;

	[System.NonSerialized]
	public int ownerID;

	private bool isReady, isThisMyObject, throwingHarpoon, owned, isStuck;
	private TNSyncHarpoon tnSync;

	private Transform ownerTrans;

	void Awake()
	{
		tnSync = GetComponent<TNSyncHarpoon>();
		tnSync.enabled = false;

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


	void SetOwner (int id) // Sender: HarpoonDetector on Player->Sprite
	{
		ownerID = id;
		owned = true;
		isStuck = false;
		harpoonTrail.DeActivateTrail();

		GameManager.Instance.playersDict.TryGetValue(id, out ownerTrans);
	
		
		gameObject.layer = harpoonOwnedLayer;
		transform.parent.transform.parent = ownerTrans;
		transform.position = ownerTrans.position;
		transform.rotation = Quaternion.identity;

		tnSync.enabled = false;	// Turn off TNSyncrigidbody2d because we are now parented to the player

		SetOwnerRemoteRFC();
	}

	void SetOwnerRemoteRFC() 
	{
		Debug.Log(ownerID);
		if(tno == null) Debug.Log("yup");
		tno.Send(60, Target.OthersSaved, ownerID);
	}

	[RFC(60)]
	void SetOwnerRemote (int id)
	{
		ownerID = id;
		owned = true;
		isStuck = false;
		harpoonTrail.DeActivateTrail();


		GameManager.Instance.playersDict.TryGetValue(id, out ownerTrans);

		gameObject.layer = harpoonOwnedLayer;
		transform.parent.transform.parent = ownerTrans;
		transform.position = ownerTrans.position;
		transform.rotation = Quaternion.identity;

		tnSync.enabled = false;
	}
	
	void Update ()
	{
		if (!isReady)
		{
			return;
		}
		if (transform.parent.transform.parent == null || gameObject.layer == harpoonFreeLayer)
		{
			return;
		}

		if (TNManager.playerID != ownerID)
		{
			return;
		}

		if(Input.GetMouseButtonDown(0) && !throwingHarpoon)
		{
			ThrowHarpoon();
		}
	}
	
	void ThrowHarpoon () 
	{

		throwingHarpoon = true;
		owned = false;
		Invoke ("ResetThrowingHarpoon", 0.5f);

		harpoonTrail.ActivateTrail();


		transform.parent.parent.SendMessage ("ThrewHarpoon", SendMessageOptions.DontRequireReceiver);  // Receiver: HarpoonDetector on Player->Sprite
		transform.parent.transform.parent = null;
		gameObject.layer = harpoonFreeLayer;
		
		Vector3 mousePos = Input.mousePosition;
		//mousePos.z = 15f; //The distance from the camera to the player object
		Ray ray = Camera.main.ScreenPointToRay(mousePos);
		Vector3 rayPos = ray.origin+(ray.direction*15f);
		Vector3 lookPos = rayPos - transform.position;
		float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
		Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);
		transform.rotation = rot;
		
		tnSync.enabled = true;

		Vector3 clickDistance = rayPos - transform.position;
		Vector2 force = clickDistance.normalized * harpoonForce;
		rigidbody2D.AddForce(force);

		tnSync.Sync();

		tno.Send(56, Target.OthersSaved, transform.position, transform.localEulerAngles);

	}

	[RFC(56)]
	void ThrowHarpoonRemote (Vector3 pos, Vector3 rot) 
	{
		throwingHarpoon = true;
		Invoke ("ResetThrowingHarpoon", 0.5f);

		harpoonTrail.ActivateTrail();


		transform.position = pos;
		transform.localEulerAngles = rot;
		tnSync.enabled = true;
		

		if (transform.parent.transform.parent != null)
		{
			transform.parent.parent.SendMessage ("ThrewHarpoon", SendMessageOptions.DontRequireReceiver);  // Receiver: HarpoonDetector on Player->Sprite
			transform.parent.transform.parent = null;
		}

		gameObject.layer = harpoonFreeLayer;

		//rigidbody2D.AddForce( force * harpoonForce);
	}

	void ResetThrowingHarpoon () { throwingHarpoon = false; }

	void OnCollisionEnter2D (Collision2D col)
	{
		if (col.gameObject.layer == 10)
		{
			Debug.Log ("hit player trigger");
			return;
		}
		if (gameObject.layer == harpoonFreeLayer && !throwingHarpoon && !isStuck) 
		{
			isStuck = true;

			harpoonTrail.DeActivateTrail();


			rigidbody2D.velocity = Vector2.zero;
			tnSync.enabled = false;
			if (TNManager.playerID == ownerID)
			{
				tno.Send(57, Target.OthersSaved, transform.position, transform.localEulerAngles);
			}
			
		}
	}

	void OnCollisionStay2D (Collision2D col)
	{
//		Debug.Log ("playerid"+TNManager.playerID+" ownerID:" + ownerID+"throwingHarpoon"+throwingHarpoon);
		if (col.gameObject.layer == 10)
		{
			Debug.Log ("hit player trigger");
			return;
		}
		if (gameObject.layer == harpoonFreeLayer && !throwingHarpoon && !isStuck) 
		{
			isStuck = true;

			harpoonTrail.DeActivateTrail();


			rigidbody2D.velocity = Vector2.zero;
			tnSync.enabled = false;
			if (TNManager.playerID == ownerID)
			{
				tno.Send(57, Target.OthersSaved, transform.position, transform.localEulerAngles);
			}
			
		}
	}
	
	[RFC(57)]
	void HarpoonStuck (Vector3 pos, Vector3 rot)
	{
//		Debug.Log(gameObject.GetInstanceID());
		rigidbody2D.velocity = Vector2.zero;
		tnSync.enabled = false;
		transform.position = pos;
		transform.rotation = Quaternion.Euler(rot);
	}

//	void OnNetworkPlayerJoin (Player player)
//	{
//		if (owned && ownerID == TNManager.playerID)
//		{
//			Invoke("SetOwnerRemoteRFC", 3f); 
//		}
//	}
}
