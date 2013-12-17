using UnityEngine;
using System.Collections;

public class Death : TNBehaviour {


	public GameObject sprite;

	public float invincibleTime;
	public float respawnTime;


	SmoothFollowPlayer camSmoothFollow;
	PlayerController playerController;
	SpawnPlayer spawner;
	bool isMine, isReady;

	void Awake () 
	{
		isMine = TNManager.isThisMyObject;
		playerController = GetComponent<PlayerController>();
		spawner = GameObject.FindGameObjectWithTag("PSpawner").GetComponent<SpawnPlayer>();
		camSmoothFollow = Camera.main.gameObject.GetComponent<SmoothFollowPlayer>();
		Invoke("SetIsReady", invincibleTime);
	}

	void SetIsReady () { isReady = true; }

	void Die () 
	{
		Debug.Log ("Death CS");
		sprite.SendMessage("Die", SendMessageOptions.DontRequireReceiver);
		rigidbody2D.velocity = Vector2.zero;
		rigidbody2D.isKinematic = true;
		playerController.enabled = false;
		camSmoothFollow.enabled = false;
	}

	void Respawn ()
	{
		transform.position = spawner.GetRandomSpawnPoint();
		sprite.SetActive (true);
		rigidbody2D.isKinematic = false;
		playerController.enabled = true;
		camSmoothFollow.enabled = true;
	}
}
