using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TNet;

public class GameManager : Singleton<GameManager> {

	protected GameManager () {} // guarantee this will be always a singleton only - can't use the constructor!

	public Dictionary<int, Transform> playersDict = new Dictionary<int, Transform>();

	private GameObject[] playersArray;

	void Awake ()
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

//	[RFC(20)] 

//	[RCC(255)]
//	static GameObject OnCreate (GameObject prefab, Vector3 pos, Quaternion rot, Vector2 moveToPos)
//	{
//		Debug.Log ("created");
//		GameObject go = Instantiate(prefab, pos, rot) as GameObject;
//		go.SendMessage("MoveToPos", moveToPos, SendMessageOptions.DontRequireReceiver);
//		return go;
//	}
//
//	public void CreateHarpoon (GameObject prefab, Vector3 pos, Quaternion rot, Vector2 moveToPos, bool persistent = false)
//	{
//		TNManager.CreateEx(255, persistent, prefab, pos, rot, moveToPos);
//		Debug.Log ("created");
//	}
}
