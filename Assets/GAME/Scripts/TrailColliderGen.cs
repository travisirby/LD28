using UnityEngine;
using System.Collections;

public class TrailColliderGen : MonoBehaviour
{
	public GameObject edgeColPrefab;
	public float trailTime = 5f;
	public float colliderGenFrequency = 0.5f;
	public int trailColliderLayer = 13;

	TrailRenderer trail;
	Vector3 lastDotPosition;
	bool lastPointExists;

	void Awake()
	{
		trail = GetComponent<TrailRenderer>();
	}

	void Start()
	{
		lastPointExists = false;
	}

	void MakeEdgeCol()
	{
		//if (lastDotPosition == transform.position) return;

		Instantiate (edgeColPrefab);

		colliderKeeper.layer = trailColliderLayer;
		//colliderKeeper.transform.position = transform.position;
		EdgeCollider2D ec = colliderKeeper.AddComponent<EdgeCollider2D>();
		//colliderKeeper.transform.position = Vector3.Lerp(transform.position, lastDotPosition, 0.5f);
		//colliderKeeper.transform.LookAt(transform.position);
		ec.points = new Vector2[2] {transform.position,lastDotPosition};

		lastDotPosition = transform.position;
		lastPointExists = true;
	}

	public void ActivateTrail ()
	{
		InvokeRepeating("MakeEdgeCol", 0.1f, colliderGenFrequency);
		trail.time = trailTime;
	}

	public void DeActivateTrail()
	{
		CancelInvoke();
		Invoke("MakeEdgeCol",0.5f);
		trail.time = 0f;
	}
}