using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrailColliderGen : MonoBehaviour
{
	public GameObject edgeColPrefab;
	public GameObject lineWavePrefab;

	public float maxTrailLength = 100f;

	public Transform harpoonBody;
	public float trailTime = 5f;
	public float colliderGenFrequency = 0.5f;
	public int trailColliderLayer = 13;

	[System.NonSerialized]
	public LineWave lineWave;
	[System.NonSerialized]
	public GameObject lineWaveObj;

	List<GameObject> edgeObjs = new List<GameObject>(10);

	GameObject spawnedEdge;
	EdgeCollider2D edgeCol;
	Vector3 lastDotPosition, lineWaveStartPos;
	bool trailActivated;
	Transform thisTrans, lineWaveTrans;

	void Awake()
	{
		thisTrans = transform;

		lineWaveObj = Instantiate (lineWavePrefab, lineWaveStartPos, harpoonBody.rotation ) as GameObject;
		lineWaveTrans = lineWaveObj.transform;
		lineWave = lineWaveObj.GetComponent<LineWave>();


	}

	void Start ()
	{
		if (lineWaveTrans != null)
		{
			lineWaveTrans.parent = harpoonBody.parent;
		}

		else 
		{
			Invoke("Start",1f);
		}
	}

	void MakeEdgeCol()
	{
//		if (lastDotPosition == thisTrans.position) return;
//
//		spawnedEdge = Instantiate (edgeColPrefab) as GameObject;
//
//		edgeCol = spawnedEdge.GetComponent<EdgeCollider2D>();
//		edgeCol.points = new Vector2[2] {thisTrans.position,lastDotPosition};
//		lastDotPosition = thisTrans.position;
	}
	
	public void ActivateTrail ()
	{
		lineWaveStartPos = thisTrans.position;
		if (lineWaveObj == null)
		{
			lineWaveObj = Instantiate (lineWavePrefab, lineWaveStartPos, harpoonBody.rotation ) as GameObject;
			
			lineWaveTrans = lineWaveObj.transform;
			
			lineWave = lineWaveObj.GetComponent<LineWave>();
		}
		else 
		{
			lineWaveTrans.position = lineWaveStartPos;
			lineWaveTrans.rotation = harpoonBody.rotation;
		}
		
		lastDotPosition = thisTrans.position;
		InvokeRepeating("MakeEdgeCol", 0.1f, colliderGenFrequency);
		
		trailActivated = true;
	}


	void FixedUpdate()
	{
		if (trailActivated)
		{
			lineWaveTrans.rotation = harpoonBody.rotation;
			if (lineWave.lengh < 50) lineWave.lengh = Vector3.Distance (thisTrans.position, lineWaveStartPos);
			lineWaveTrans.position = Vector3.Lerp (lineWaveTrans.position, thisTrans.position, 0.25f);
		}
		else if (lineWaveObj == null) 
		{
			return;
		}
		else if (!trailActivated && lineWave.lengh > 0f)
		{
			lineWave.lengh -= 2f;
			lineWaveTrans.position = Vector3.Lerp (lineWaveTrans.position, thisTrans.position, 0.25f);
		}
		else
		{
			lineWaveTrans.position = thisTrans.position;
			lineWaveTrans.rotation = Quaternion.Lerp (lineWaveTrans.rotation, harpoonBody.rotation, Time.fixedDeltaTime * 5f);
		}
	}

	public void SetLengh (float length)
	{
		if (lineWave != null) 
		{
			lineWave.lengh = length;
		}
	}


	public void FreezeTrail()
	{
		CancelInvoke();

		if (trailActivated)
		{
			//lineWaveTrans.rotation = Quaternion.Inverse(lineWaveTrans.rotation);
			//trailActivated = false;
			Invoke("MakeEdgeCol",0.5f);
		}
	}

	public void CollapseTrail ()
	{
		CancelInvoke();

		if (trailActivated)
		{
			trailActivated = false;
			//Invoke("MakeEdgeCol",0.5f);
		}
	}
}