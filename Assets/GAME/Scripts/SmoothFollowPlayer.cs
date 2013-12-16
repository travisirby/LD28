using UnityEngine;
using System.Collections;

public class SmoothFollowPlayer : MonoBehaviour {
	
	public Transform target;
	public bool smoothRotate;

	public float lookAheadRateX = 10f;
	public float lookAheadRateY = 10f;

	public float rotationSpeed;
	[Range(0.1f, 20f)] public float followRate = 10;

	Transform thisTransform;
	Rigidbody2D playerRB;
	bool isReady;
	float thisOldPosX, thisOldPosY, targetOldPosX, targetOldPosY, oldLookAheadX, oldLookAheadY; 

	void Awake ()
	{
		thisTransform = transform;
	}
	
	void SetupPlayerCam (Transform trans)
	{
		target = trans;

		Invoke("Ready",1f);
	}

	void SetupPlayerRigidbody (Rigidbody2D rb)
	{
		playerRB = rb;
	}

	void Ready ()
	{
		thisOldPosX = thisTransform.position.x;
		thisOldPosY = thisTransform.position.y;
		targetOldPosX = target.position.x;
		targetOldPosY = target.position.y;

		isReady = true;
	}

	void FixedUpdate ()
	{
		if (isReady)
		{
			Vector2 playerVelNorm = playerRB.velocity.normalized;
			float lookAheadX = lookAheadRateX * playerVelNorm.x;
			float lookAheadY = lookAheadRateY * playerVelNorm.y;

			float thisTransformPosX = SuperSmoothLerp(thisOldPosX, targetOldPosX, target.position.x + lookAheadX, Time.smoothDeltaTime, followRate);
			float thisTransformPosY = SuperSmoothLerp(thisOldPosY, targetOldPosY, target.position.y + lookAheadY, Time.smoothDeltaTime, followRate);

			Vector3 newTransPos = new Vector3 (thisTransformPosX, thisTransformPosY, thisTransform.position.z);
			thisTransform.position = newTransPos;

			oldLookAheadX = lookAheadX;
			oldLookAheadY = lookAheadY;
			thisOldPosX = transform.position.x;
			thisOldPosY = transform.position.y;
			targetOldPosX = target.position.x;
			targetOldPosY = target.position.y;

			if (smoothRotate)
			{
				transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * rotationSpeed);
			}
		}
	}

	float SuperSmoothLerp (float thisOld, float targetOld, float targetNew, float time, float rate){
		float f = thisOld - targetOld + (targetNew - targetOld) / (rate * time);
		return targetNew - (targetNew - targetOld) / (rate*time) + f * Mathf.Exp(-rate*time);
	}
}
