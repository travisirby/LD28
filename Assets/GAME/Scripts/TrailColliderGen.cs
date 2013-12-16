using UnityEngine;
using System.Collections;

public class TrailColliderGen : MonoBehaviour
{
	public GameObject edgeColPrefab;
	public GameObject lineWavePrefab;
	public Transform harpoonBody;
	public float trailTime = 5f;
	public float colliderGenFrequency = 0.5f;
	public int trailColliderLayer = 13;

	TrailRenderer trail;
	GameObject spawnedEdge, lineWaveObj;
	LineWave lineWave;
	EdgeCollider2D edgeCol;
	Vector3 lastDotPosition, lineWaveStartPos;
	bool trailActivated;
	Transform thisTrans, lineWaveTrans;

	void Awake()
	{
	//	trail = GetComponent<TrailRenderer>();
	//	trail.time = trailTime;
		thisTrans = transform;
	}

	void MakeEdgeCol()
	{
		if (lastDotPosition == thisTrans.position) return;

		spawnedEdge = Instantiate (edgeColPrefab) as GameObject;

		edgeCol = spawnedEdge.GetComponent<EdgeCollider2D>();
		edgeCol.points = new Vector2[2] {thisTrans.position,lastDotPosition};
		lastDotPosition = thisTrans.position;
	}
	
	public void ActivateTrail ()
	{
		lineWaveStartPos = harpoonBody.position;
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
			lineWave.lengh = Vector3.Distance (thisTrans.position, lineWaveStartPos);
			lineWaveTrans.position = Vector3.Lerp (lineWaveTrans.position, thisTrans.position, 0.25f);
		}
		else if (lineWaveObj == null) 
		{
			return;
		}
		else if (!trailActivated &&lineWave.lengh >= 8f)
		{
			lineWave.freq -= 0.01f;
			lineWave.lengh -= 1f;
			lineWaveTrans.position = Vector3.Lerp (lineWaveTrans.position, thisTrans.position, 0.25f);
		}
		else
		{
			lineWaveTrans.position = thisTrans.position;
			lineWaveTrans.rotation = Quaternion.Lerp (lineWaveTrans.rotation, harpoonBody.rotation, Time.fixedDeltaTime * 5f);
		}
	}


	public void DeActivateTrail()
	{
		CancelInvoke();
		if (trailActivated)
		{
			//lineWaveTrans.rotation = Quaternion.Inverse(lineWaveTrans.rotation);
			//trailActivated = false;
			Invoke("MakeEdgeCol",0.5f);
		}
	}

	public void HideTrail ()
	{		
		CancelInvoke();
		if (trailActivated)
		{
			trailActivated = false;
			//Invoke("MakeEdgeCol",0.5f);
		}
	}
}