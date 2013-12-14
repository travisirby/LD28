using UnityEngine;
using System.Collections;

public class HarpoonLogic : MonoBehaviour {

	public float harpoonForce = 100;

	void MoveToPos (Vector2 pos)
	{
		rigidbody2D.AddForce(pos.normalized*harpoonForce);
	}
}
