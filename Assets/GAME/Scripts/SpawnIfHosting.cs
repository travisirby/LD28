using UnityEngine;
using System.Collections;
using TNet;
public class SpawnIfHosting : MonoBehaviour {

	public GameObject prefabToSpawn;
	public bool persistent;

	void OnNetworkJoinChannel (bool result, string message)
	{
		if (result)
		{
			if (TNManager.isHosting) SpawnNow();
		}
	}

	void SpawnNow () 
	{
		TNManager.Create(prefabToSpawn, transform.position, Quaternion.identity, persistent);
	}
}
