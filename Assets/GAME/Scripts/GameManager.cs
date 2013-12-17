using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TNet;

public class GameManager : Singleton<GameManager> {

	protected GameManager () {} // guarantee this will be always a singleton only - can't use the constructor!

	public Dictionary<int, Transform> playersDict = new Dictionary<int, Transform>();

	public AudioSource audioLaser;				// Text_Typewriter calls this during typing effect
	public AudioSource audioDie;			

	GameObject[] playersArray;
	bool addPlayersRunning;

	void Start ()
	{
		StartAddPlayersToDict();
		AudioSource[] aSources = GetComponents<AudioSource>();
		audioLaser = aSources[0];
		audioDie = aSources[1];
	}

	public void StartAddPlayersToDict ()
	{
		if (!addPlayersRunning)
		{
			StartCoroutine("AddPlayersToDict");
		}
	}

	public void PlaySoundLaser()				// Text_Typewriter calls this during typing effect
	{
		audioLaser.Play ();
	}
	public void PlaySoundDie()				// Text_Typewriter calls this during typing effect
	{
		audioDie.Play();
	}
	IEnumerator AddPlayersToDict ()
	{
		addPlayersRunning = true;
		playersArray = GameObject.FindGameObjectsWithTag("Player");
		playersDict.Clear();
		foreach (GameObject player in playersArray) 
		{
			int ownerID = player.gameObject.GetComponent<HarpoonDetector>().ownerID;
			if (!playersDict.ContainsKey(ownerID))
			{
				playersDict.Add (ownerID, player.transform);
			}
			Debug.Log("InCoLoop");

			yield return null;
		}
		Debug.Log("DoneCo");

		addPlayersRunning = false;
	}
	
	void OnNetworkPlayerJoin (Player player)
	{
		Debug.Log (player.name);
		StartAddPlayersToDict();
	}

	void OnNetworkPlayerLeave (Player player)
	{
		StartAddPlayersToDict();
	}
	
	
	[RCC(255)]
	static GameObject CreateTheHarpoon (GameObject prefab, Vector3 pos, Quaternion rot, int ownerID)
	{
		Debug.Log ("created2");
		GameObject go = Instantiate(prefab, pos, rot) as GameObject;
		go.transform.GetChild(0).gameObject.GetComponent<HarpoonThrow>().SetOwner(ownerID);
		return go;
	}

	public void CreateHarpoon (GameObject prefab, Vector3 pos, Quaternion rot, int ownerID, bool persistent = true)
	{
		TNManager.CreateEx(255, persistent, prefab, pos, rot, ownerID);
		Debug.Log ("created");
	}
}
