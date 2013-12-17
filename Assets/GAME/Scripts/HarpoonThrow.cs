using UnityEngine;
using System.Collections;
using TNet;

public class HarpoonThrow : TNBehaviour {

	public TrailColliderGen harpoonTrail;
	public float harpoonForce = 200, blowBackForce = 100f;
	public int harpoonFreeLayer = 11;
	public int harpoonOwnedLayer = 12;

	[System.NonSerialized]
	public int ownerID;

	bool isReady, isOwned, isThisMyObject, throwingHarpoon, isStuck;
	TNSyncHarpoon tnSync;

	private Transform ownerTrans;

	void Awake()
	{
		if (TNManager.isThisMyObject)
		{
			isThisMyObject = true;
		}
		tnSync = GetComponent<TNSyncHarpoon>();
		tnSync.enabled = false;
	}

	void Start ()
	{
		Invoke("SetIsReady",2f);
	}
	
	void SetIsReady () { isReady = true; }

	void SetOwnerRetry () { SetOwner(ownerID); }

	public void SetOwner (int id) // Sender: HarpoonDetector on Player->Sprite
	{
		ownerID = id;
		isOwned = true;
		isStuck = false;

		harpoonTrail.CollapseTrail();

		GameManager.Instance.playersDict.TryGetValue(id, out ownerTrans);

		if (ownerTrans == null)
		{
			Debug.Log(id);
			GameManager.Instance.StartAddPlayersToDict();
			Invoke("SetOwnerRetry", 1f);
			return;
		}
		rigidbody2D.velocity = Vector2.zero;
		rigidbody2D.isKinematic = true;

		harpoonTrail.StartCoroutine("ColorLerpToBlue");

		gameObject.layer = harpoonOwnedLayer;
		transform.parent.transform.parent = ownerTrans;
		transform.position = new Vector3 (ownerTrans.position.x, ownerTrans.position.y + 1f, 0f);
		transform.rotation = ownerTrans.localRotation;

		tnSync.enabled = false;	// Turn off TNSyncrigidbody2d because we are now parented to the player

		tno.Send(60, Target.Others, ownerID);

	}
	
	void SetOwnerRemoteRetry () { SetOwnerRemote(ownerID); }
	
	[RFC(60)]
	void SetOwnerRemote (int id)
	{

		Debug.Log (TNManager.playerID);
		ownerID = id;
		isStuck = false;
	
		harpoonTrail.CollapseTrail();

		GameManager.Instance.playersDict.TryGetValue(id, out ownerTrans);

		if (ownerTrans == null)
		{
			Debug.Log(id + "remote");
			GameManager.Instance.StartAddPlayersToDict();
			Invoke("SetOwnerRemoteRetry", 0.5f);
			return;
		}

		harpoonTrail.StartCoroutine("ColorLerpToBlue");

		rigidbody2D.isKinematic = true;

		gameObject.layer = harpoonOwnedLayer;

		transform.parent.transform.parent = ownerTrans;

		transform.position = new Vector3 (ownerTrans.position.x, ownerTrans.position.y + 1f, 0f);

		transform.rotation = ownerTrans.localRotation;

		tnSync.enabled = false;
	}
	
	void Update ()
	{
		if(Input.GetMouseButtonDown(0) && !throwingHarpoon)
		{
			if (!isReady || gameObject.layer == harpoonFreeLayer ||transform.parent.parent == null) return;

			if (TNManager.playerID == ownerID) ThrowHarpoon();
		}
	}
	
	void ThrowHarpoon () 
	{
		throwingHarpoon = true;
		isOwned = false;
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

		GameManager.Instance.PlaySoundLaser();

		tnSync.enabled = true;

		Vector3 clickDistance = rayPos - transform.position;
		Vector2 force = clickDistance.normalized * harpoonForce;

		rigidbody2D.isKinematic = false;
		rigidbody2D.AddForce(force);
//
//		if (ownerTrans != null) 
//		{
//			ownerTrans.rigidbody2D.velocity = -force * blowBackForce;
//		}

		tnSync.Sync();

		tno.Send(56, Target.Others, transform.position, transform.localEulerAngles);
	}

	[RFC(56)]
	void ThrowHarpoonRemote (Vector3 pos, Vector3 rot) 
	{
		throwingHarpoon = true;
		isOwned = false;
		Invoke ("ResetThrowingHarpoon", 0.5f);

		transform.position = pos;
		transform.localEulerAngles = rot;

		GameManager.Instance.PlaySoundLaser();

		harpoonTrail.ActivateTrail();

		rigidbody2D.isKinematic = false;

		tnSync.enabled = true;
		

		if (transform.parent.transform.parent != null)
		{
			transform.parent.transform.parent.SendMessage ("ThrewHarpoon", SendMessageOptions.DontRequireReceiver);  // Receiver: HarpoonDetector on Player->Sprite
			transform.parent.transform.parent = null;
		}

		gameObject.layer = harpoonFreeLayer;

	}

	void ResetThrowingHarpoon () { throwingHarpoon = false; }

	void OnCollisionEnter2D (Collision2D col)
	{
		if (col.gameObject.layer == 10)
		{
			Debug.Log ("hit player trigger");
			return;
		}
		if (col.gameObject.layer == 8 && gameObject.layer == harpoonFreeLayer && !throwingHarpoon && !isStuck) 
		{
			isStuck = true;

			harpoonTrail.FreezeTrail();

			rigidbody2D.velocity = Vector2.zero;
			tnSync.enabled = false;

			if (TNManager.playerID == ownerID)
			{
				tno.Send(57, Target.Others, transform.position, harpoonTrail.lineWave.lengh, true);
				harpoonTrail.StartCoroutine("ColorLerpToRed");
				
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
		if (col.gameObject.layer == 8 && gameObject.layer == harpoonFreeLayer && !throwingHarpoon && !isStuck) 
		{
			isStuck = true;

			harpoonTrail.FreezeTrail();


			rigidbody2D.velocity = Vector2.zero;
			tnSync.enabled = false;
			if (TNManager.playerID == ownerID)
			{
				tno.Send(57, Target.Others, transform.position, harpoonTrail.lineWave.lengh, true);
				harpoonTrail.StartCoroutine("ColorLerpToRed");

			}
			
		}
	}
	
	[RFC(57)]
	void HarpoonStuck (Vector3 pos, float trailLength, bool isStuck)
	{
		if (isStuck)
		{
			rigidbody2D.velocity = Vector2.zero;
			tnSync.enabled = false;
			transform.position = pos;
			harpoonTrail.SetLengh (trailLength);
			harpoonTrail.StartCoroutine("ColorLerpToRed");
		}
	}

	void OnNetworkPlayerJoin (Player player)
	{
		Debug.Log (player.id);
		if (ownerTrans.gameObject.GetComponent<HarpoonDetector>().isThisMyObject)
		{
			if (TNManager.playerID == ownerID)
			{
				if (isStuck)
				{
					tno.Send(57,  Target.Others, transform.position, harpoonTrail.lineWave.lengh, true);		// Here I'm clearing the saved rfc that thinks the harpoon is still owned
				}
				else if (isOwned)
				{
					tno.Send(60, Target.Others, ownerID);
				}

			}
		}
	}

	void OnNetworkPlayerLeave (Player player)
	{

		if (player.id == ownerID)
		{
			Debug.Log (player.id + "IM IN");
			TNManager.Destroy(transform.parent.gameObject);

		}
	}

}
