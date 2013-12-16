using UnityEngine;
using System.Collections;

public class HarpoonSpawn : MonoBehaviour {

	public GameObject harpoonPrefab;

	private bool isThisMyObject;

	void Awake ()
	{
		if (TNManager.isThisMyObject)
		{
			isThisMyObject = true;
		}
	}

	void Start ()
	{
		if (isThisMyObject)
		{
			SpawnHarpoon();
		}
	}

	void SpawnHarpoon () 
	{
		GameManager.Instance.CreateHarpoon(harpoonPrefab, transform.position, Quaternion.identity, TNManager.playerID, true);
	}
}
