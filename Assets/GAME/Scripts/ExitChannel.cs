using UnityEngine;
using System.Collections;

public class ExitChannel : MonoBehaviour {

	void Update ()
	{
		if (Input.GetMouseButtonUp(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit2D hit = Physics2D.GetRayIntersection(ray,Mathf.Infinity);
			
			if(hit.collider != null && hit.collider.transform == transform)
			{
				Debug.Log ("Closed Channel");
				TNManager.CloseChannel();
			}
		}
	}
}
