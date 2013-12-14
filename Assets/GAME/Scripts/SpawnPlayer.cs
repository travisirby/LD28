using UnityEngine;
using System.Collections;
using TNet;
public class SpawnPlayer : MonoBehaviour {

	public GameObject playerPrefab;
	public Transform spawnPos;

	void OnNetworkJoinChannel (bool result, string message)
	{
		if (result)
		{
			SpawnPlayerNow();
		}
	}

	void SpawnPlayerNow () 
	{
		TNManager.Create(playerPrefab, spawnPos.position, Quaternion.identity, false);
	}
}
