using UnityEngine;
using System.Collections;

public class SmoothFollowPlayer : MonoBehaviour {
	
	public Transform target;
	public bool smoothRotate;
	public float rotationSpeed;
	[Range(0.1f, 20f)] public float followRate = 10;

	Transform thisTransform;
	bool isReady;
	float thisOldPosX, thisOldPosY, targetOldPosX, targetOldPosY; 

	void Awake ()
	{
		thisTransform = transform;
	}
	
	void SetupPlayerCam (Transform trans)
	{
		target = trans;
		Invoke("Ready",0.1f);
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

			float thisTransformPosX = SuperSmoothLerp(thisOldPosX, targetOldPosX, target.position.x, Time.smoothDeltaTime, followRate);
			float thisTransformPosY = SuperSmoothLerp(thisOldPosY, targetOldPosY, target.position.y, Time.smoothDeltaTime, followRate);

			Vector3 newTransPos = new Vector3 (thisTransformPosX, thisTransformPosY, thisTransform.position.z);
			thisTransform.position = newTransPos;

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
