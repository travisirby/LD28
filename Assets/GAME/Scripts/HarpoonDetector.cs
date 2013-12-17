using UnityEngine;
using System.Collections;

public class HarpoonDetector : MonoBehaviour {

	public int harpoonOwnedLayer = 12;

	[System.NonSerialized]
	public int ownerID;
	[System.NonSerialized]
	public bool isThisMyObject;

	bool isReady, holdingHarpoon = true, throwingHarpoon;

	void Awake()
	{
		if (TNManager.isThisMyObject)
		{
			isThisMyObject = true;
		}
		ownerID = TNManager.objectOwnerID;
		Invoke("SetIsReady", 2f);
	}

	void SetIsReady () { isReady = true; }
	

	void ThrewHarpoon ()			// Sender is HarpoonThrower
	{
		holdingHarpoon = false;
		throwingHarpoon = true;
		Invoke ("ResetThrowingHarpoon", 3f);
	}

	void ResetThrowingHarpoon () { throwingHarpoon = false; }

	void OnTriggerEnter2D (Collider2D col)
	{
		if (!isThisMyObject || !isReady) return;  // Is THIS player our object?? Does not mean the harpoon we are colliding with 

		int trailColliderLayer = 13;
		if (col.gameObject.layer == trailColliderLayer)
		{

			transform.parent.BroadcastMessage("Die",SendMessageOptions.DontRequireReceiver);
		}

		else if (col.CompareTag("Harpoon"))
		{
			if (!holdingHarpoon && !throwingHarpoon && col.gameObject.layer != harpoonOwnedLayer)
			{
				if (col.rigidbody2D.velocity.magnitude > 0.1f)
				{
					return;
				}
				holdingHarpoon = true;
				col.transform.SendMessage ("SetOwner", TNManager.playerID);
			}
		}
	}

	void OnTriggerStay2D (Collider2D col)
	{
		if (!isThisMyObject || !isReady) return;  // Is THIS player our object?? Does not mean the harpoon we are colliding with 
		
		if (col.CompareTag("Harpoon"))
		{
			if (!holdingHarpoon && !throwingHarpoon && col.gameObject.layer != harpoonOwnedLayer)
			{
				if (col.rigidbody2D.velocity.magnitude > 0.1f)
				{
					return;
				}
				holdingHarpoon = true;
				col.transform.SendMessage ("SetOwner", TNManager.playerID);
			}
		}
	}


//	[RFC(36)]
//	void OnSync (Vector3 pos, Vector3 rot, Vector2 vel)
//	{
//		
//	}
//
//	void OnNetworkPlayerJoin (Player player)
//	{
//		tno.Send(36, Target.Others, mLastPos, mLastRot, mRb.velocity);
//	}
}
