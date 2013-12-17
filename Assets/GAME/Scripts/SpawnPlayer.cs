using UnityEngine;
using System.Collections;
using TNet;
public class SpawnPlayer : MonoBehaviour {

	public GameObject playerPrefab;

	public float spawnDistanceX = 30f;
	public float spawnDistanceY = 30f;

	void OnNetworkJoinChannel (bool result, string message)
	{
		if (result)
		{
			SpawnPlayerNow();
		}
	}

	void SpawnPlayerNow () 
	{
		float randomX = Random.Range(-spawnDistanceX, spawnDistanceX);
		float randomY = Random.Range(-spawnDistanceY, spawnDistanceY);

		Vector3 spawnPos = new Vector3 (transform.position.x+randomX, transform.position.y+randomY, 0f);

		TNManager.Create(playerPrefab, spawnPos, Quaternion.identity, false);
		GameManager.Instance.Invoke("AddPlayersToDict", 1f);  // CHANGED FROM 3f
	}

	public Vector3 GetRandomSpawnPoint ()
	{
		float randomX = Random.Range(-spawnDistanceX, spawnDistanceX);
		float randomY = Random.Range(-spawnDistanceY, spawnDistanceY);
		
		Vector3 spawnPos = new Vector3 (transform.position.x+randomX, transform.position.y+randomY, 0f);

		return spawnPos;
	}
	
}
