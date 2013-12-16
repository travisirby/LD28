using UnityEngine;
using System.Collections;

public class PlayerSpriteRotate : MonoBehaviour {

	public float rotateRate;

	Transform thisTrans;
	Rigidbody2D playerBody;
	bool isReady, isMine;
	Quaternion previousRot;


	void Awake ()
	{
		if (TNManager.isThisMyObject)
		{
			isMine = true;
		}

		thisTrans = transform;
		playerBody = GameObject.FindGameObjectWithTag("PBody").GetComponent<Rigidbody2D>();
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

//		if (Mathf.Abs (playerVelocity.normalized.x) < 0.1f)
//		{
//			angle = 90f;
//			rot = Quaternion.AngleAxis(angle, Vector3.forward);
//			thisTrans.rotation = Quaternion.Lerp (thisTrans.localRotation, rot, 1f * Time.fixedDeltaTime);
//		}
//		else 
//		{
		 	rot = Quaternion.AngleAxis(angle, Vector3.forward);
			thisTrans.rotation = Quaternion.Lerp (thisTrans.localRotation, rot, rotateRate * Time.fixedDeltaTime);
		}

}
