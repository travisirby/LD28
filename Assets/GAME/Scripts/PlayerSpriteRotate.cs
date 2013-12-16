using UnityEngine;
using System.Collections;

public class PlayerSpriteRotate : MonoBehaviour {

	public float rotateRate;
	public Rigidbody2D playerBody;


	Transform thisTrans;
	bool isReady, isMine;
	Quaternion previousRot;


	void Awake ()
	{
		if (TNManager.isThisMyObject)
		{
			isMine = true;
		}

		thisTrans = transform;
		Invoke("SetIsReady", 2f);
	}

	void SetIsReady () { isReady = true; }
	
	void FixedUpdate ()
	{
		if (!isReady) return;

		Vector3 playerVelocity = playerBody.velocity;



		Vector2 moveToPos = transform.position + (playerVelocity);



		float angle = Mathf.Atan2 (playerVelocity.normalized.y, playerVelocity.normalized.x) * Mathf.Rad2Deg;

		Quaternion rot = Quaternion.identity;

		if (Mathf.Abs (playerVelocity.magnitude) < 0.1f)
		{
			angle = 90f;
			rot = Quaternion.AngleAxis(angle, Vector3.forward);
			thisTrans.rotation = Quaternion.Lerp (thisTrans.localRotation, rot, rotateRate * Time.fixedDeltaTime);
		}
		else 
		{
		 	rot = Quaternion.AngleAxis(angle, Vector3.forward);
			thisTrans.rotation = Quaternion.Lerp (thisTrans.localRotation, rot, rotateRate * Time.fixedDeltaTime);
		}
	}

}
