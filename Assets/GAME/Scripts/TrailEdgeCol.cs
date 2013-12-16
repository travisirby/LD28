using UnityEngine;
using System.Collections;

public class TrailEdgeCol : MonoBehaviour {

	public float edgeLifetime = 5f;

	void Start () 
	{
		Invoke("DestroyThis", edgeLifetime);
	}
	
	void DestroyThis ()
	{
		Destroy(gameObject);
	}
}
