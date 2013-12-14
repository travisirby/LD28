using UnityEngine;
using System.Collections;

public class ThrowHarpoon : MonoBehaviour {

	public GameObject harpoonPrefab;
	public Transform sprite;

	private bool isThisMyObject;

	void Awake()
	{
		if (TNManager.isThisMyObject)
		{
			isThisMyObject = true;
		}
	}
	
	void Update ()
	{
		if(isThisMyObject && Input.GetMouseButtonDown(0))
		{
			Debug.Log ("Clicked");
			Vector3 clickDistance = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
			Quaternion clickRotate = Quaternion.Euler(clickDistance.normalized);
			GameManager.Instance.CreateHarpoon (harpoonPrefab, transform.position, Quaternion.LookRotation(clickDistance.normalized,Vector3.right), Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position, false);
	//		TNManager.Create(harpoonPrefab, transform.position, Quaternion.identity, false);
		}
	}
}
