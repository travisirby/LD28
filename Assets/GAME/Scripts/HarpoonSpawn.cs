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
		TNManager.Create(harpoonPrefab, transform.position, Quaternion.identity, false);
	}
}
