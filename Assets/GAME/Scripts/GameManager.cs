using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TNet;

public class GameManager : Singleton<GameManager> {

	protected GameManager () {} // guarantee this will be always a singleton only - can't use the constructor!

	public Dictionary<int, Transform> playersDict = new Dictionary<int, Transform>();

	private GameObject[] playersArray;

	void Start ()
	{
		AddPlayersToDict();
	}

	public void AddPlayersToDict ()
	{
		playersArray = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject player in playersArray) 
		{
			int ownerID = player.gameObject.GetComponent<HarpoonDetector>().ownerID;
			if (!playersDict.ContainsKey(ownerID))
			{
				playersDict.Add (ownerID, player.transform);
			}
		}
	}

	void OnNetworkPlayerJoin (Player player)
	{
		Debug.Log (player.name);
		AddPlayersToDict ();
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
