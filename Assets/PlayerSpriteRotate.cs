using UnityEngine;
using System.Collections;

public class PlayerSpriteRotate : MonoBehaviour {

	public float rotateRate;

	Transform thisTrans;
	Rigidbody2D playerBody;
	bool isReady;
	Quaternion previousRot;

	void Start () 
	{
		thisTrans = transform;
		playerBody = GameObject.FindGameObjectWithTag("PlayerBody").GetComponent<Rigidbody2D>();
		Invoke("SetIsReady", 2f);
	}

	void SetIsReady () { isReady = true; }
	
	void FixedUpdate ()
	{
		if (!isReady) return;

		Vector3 playerVelocity = playerBody.velocity;


		Vector2 moveToPos = transform.position + (playerVelocity);



		float angle = Mathf.Atan2 (playerVelocity.normalized.y, playerVelocity.normalized.x) * Mathf.Rad2Deg;
	
		Debug.Log (angle);
		Debug.DrawLine (thisTrans.position, moveToPos, Color.red, 2f);
		//if (angle < -40f || angle > 40f) return;
	
		Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);


		thisTrans.rotation = Quaternion.Lerp (thisTrans.localRotation, rot, rotateRate * Time.fixedDeltaTime);
		




	}
}
