using UnityEngine;
using System.Collections;
using TNet;

public class HarpoonThrow : TNBehaviour {

	public SmoothFollow spriteFollow;
	
	public float harpoonForce = 200;
	public int harpoonFreeLayer = 11;
	public int harpoonOwnedLayer = 12;

	[System.NonSerialized]
	public int ownerID;

	private bool isReady, isThisMyObject, throwingHarpoon, owned;
	private TNSyncHarpoon tnSync;

	private Transform ownerTrans;

	void Awake()
	{
		tnSync = GetComponent<TNSyncHarpoon>();
		tnSync.enabled = false;

		if (TNManager.isThisMyObject)
		{
			isThisMyObject = true;
		 	InvokeRepeating("SyncHarpoon",2f,0.5f);
		}
	}

	void Start ()
	{
		Invoke("SetIsReady",2f);
//		Invoke("DisableSpriteFollow",2f);
	}

	void SetIsReady () { isReady = true; }

	void DisableSpriteFollow () { spriteFollow.enabled = false; }

	void SetOwner (int id) // Sender: HarpoonDetector on Player->Sprite
	{
		ownerID = id;
		owned = true;
		tnSync.enabled = false;	// Turn off TNSyncrigidbody2d because we are now parented to the player

		GameManager.Instance.playersDict.TryGetValue(id, out ownerTrans);
	
		
		gameObject.layer = harpoonOwnedLayer;
		transform.parent.transform.parent = ownerTrans;
		transform.position = ownerTrans.position;
		transform.rotation = Quaternion.identity;

		SetOwnerRemoteRFC();
	}

	void SetOwnerRemoteRFC() 
	{
		tno.Send(55, Target.OthersSaved, ownerID);
	}

	[RFC(55)]
	void SetOwnerRemote (int id)
	{
		Debug.Log("SETUP");
		ownerID = id;
		owned = true;
		GameManager.Instance.playersDict.TryGetValue(id, out ownerTrans);

		tnSync.enabled = false;
	//	spriteFollow.enabled = false;

		gameObject.layer = harpoonOwnedLayer;
		transform.parent.transform.parent = ownerTrans;
		transform.position = ownerTrans.position;
		transform.rotation = Quaternion.identity;
//		spriteFollow.transform.rotation = Quaternion.identity;
	}
	
	void Update ()
	{
		if (transform.parent.transform.parent == null || gameObject.layer == harpoonFreeLayer)
		{
			return;
		}

		if (TNManager.playerID != ownerID)
		{
			return;
		}

		if(Input.GetMouseButtonDown(0) && isReady && !throwingHarpoon)
		{
			ThrowHarpoon();
		}
	}
	
	void ThrowHarpoon () 
	{

		throwingHarpoon = true;
		owned = false;
		Invoke ("ResetThrowingHarpoon", 0.5f);
		

		
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
		
		//Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3 clickDistance = rayPos - transform.position;
		Vector2 force = clickDistance.normalized * harpoonForce;
		rigidbody2D.AddForce(force);
		Debug.Log (harpoonForce);
		//	spriteFollow.enabled = true;
		tnSync.enabled = true;
		tno.Send(56, Target.OthersSaved, force, transform.localEulerAngles);

	}

	[RFC(56)]
	void ThrowHarpoonRemote (Vector2 pos, Vector3 rot) 
	{
		throwingHarpoon = true;
		Invoke ("ResetThrowingHarpoon", 0.5f);
		
	//	spriteFollow.enabled = true;
		tnSync.enabled = true;
		

		if (transform.parent.transform.parent != null)
		{
			transform.parent.parent.SendMessage ("ThrewHarpoon", SendMessageOptions.DontRequireReceiver);  // Receiver: HarpoonDetector on Player->Sprite
			transform.parent.transform.parent = null;
		}

		gameObject.layer = harpoonFreeLayer;

		transform.position = pos;
		transform.localEulerAngles = rot;

		//rigidbody2D.AddForce( force * harpoonForce);

	}

	void ResetThrowingHarpoon () { throwingHarpoon = false; }

	void OnCollisionEnter2D (Collision2D col)
	{
		Debug.Log ("playerid"+TNManager.playerID+" ownerID:" + ownerID+"throwingHarpoon"+throwingHarpoon);
		if (TNManager.playerID == ownerID && !throwingHarpoon) 
		{
			Debug.Log ("stopped");
			rigidbody2D.velocity = Vector2.zero;
		//	rigidbody2D.Sleep();

			tno.Send(56, Target.OthersSaved, transform.position, transform.localEulerAngles);
		}
	}

	void OnCollisionStay2D (Collision2D col)
	{
		Debug.Log ("playerid"+TNManager.playerID+" ownerID:" + ownerID+"throwingHarpoon"+throwingHarpoon);
		if (TNManager.playerID == ownerID && !throwingHarpoon) 
		{
			Debug.Log ("stopped");
			rigidbody2D.velocity = Vector2.zero;
		//	rigidbody2D.Sleep();
			
			tno.Send(56, Target.OthersSaved, transform.position, transform.localEulerAngles);
		}
	}

	void SyncHarpoon()
	{
		if (gameObject.layer == harpoonFreeLayer)
		{
			if (!tnSync.enabled) tnSync.enabled = true;
		}
		else if (TNManager.isInChannel)
		{
			if (tnSync.enabled) tnSync.enabled = false;
			//tno.Send(100, Target.OthersSaved, transform.position, transform.eulerAngles);
		}
	}
	
	[RFC(100)]
	void OnSync (Vector3 pos, Vector3 rot)
	{
		Debug.Log(gameObject.GetInstanceID());
		transform.position = pos;
		transform.rotation = Quaternion.Euler(rot);
	}

	void OnNetworkPlayerJoin (Player player)
	{
		if (owned && ownerID == TNManager.playerID)
		{
			Invoke("SetOwnerRemoteRFC", 1f); 
		}
	}
}
