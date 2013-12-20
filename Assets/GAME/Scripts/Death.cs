using UnityEngine;
using System.Collections;
using TNet;

public class Death : TNBehaviour {


	public GameObject sprite;
	public GameObject deathParticle;
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
		if (!isReady) return;
		Debug.Log ("Death CS");
		Instantiate (deathParticle, transform.position, Quaternion.identity);
		rigidbody2D.velocity = Vector2.zero;
		rigidbody2D.isKinematic = true;

		GameManager.Instance.PlaySoundDie();

		sprite.SetActive (false);
		transform.position = spawner.GetRandomSpawnPoint();
		playerController.enabled = false;
		camSmoothFollow.enabled = false;
		Invoke("Respawn", respawnTime);
		isReady = false;
		tno.Send(116, Target.Others, transform.position);

	}

	void Respawn ()
	{
		isReady = false;
		sprite.SetActive (true);
		rigidbody2D.isKinematic = false;
		playerController.enabled = true;
		camSmoothFollow.enabled = true;
		Invoke("SetIsReady", invincibleTime);
		tno.Send(117, Target.Others);
	}



	[RFC(116)]
	void DieRemote (Vector3 pos) 
	{
		Instantiate (deathParticle, transform.position, Quaternion.identity);

		rigidbody2D.velocity = Vector2.zero;
		rigidbody2D.isKinematic = true;
		GameManager.Instance.PlaySoundDie();
		sprite.SetActive (false);
	}


	[RFC(117)]
	void RespawnRemote () 
	{
		sprite.SetActive (true);
		rigidbody2D.isKinematic = false;
		Invoke("SetIsReady", invincibleTime);
	}
}
