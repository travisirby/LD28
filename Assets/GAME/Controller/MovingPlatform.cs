using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {

	public float moveDistance = 50f;
	public float moveSpeed = 20f;

	private Rigidbody2D rb2D;
	private Vector3 originalPosition;
	private bool isReady;

	void Start () 
	{
		rb2D = rigidbody2D;			// Cache rigidbody2D for performance reasons
		originalPosition = transform.position;
		Invoke("SetIsReady", 0.5f);
	}

	void SetIsReady () { isReady = true; }

//	void Update () 
//	{
//		transform.position = new Vector3 (transform.position.x, originalPosition.y, originalPosition.z);
//	}

	void FixedUpdate () 
	{
		if (isReady)
		{
			if (rb2D.velocity.x > 0f)
			{
				if (originalPosition.x + moveDistance > transform.position.x)
					rb2D.AddForce ( new Vector2 (moveSpeed, 0f));
				else
					rb2D.AddForce ( new Vector2 (-moveSpeed, 0f));
			}
			else if (rb2D.velocity.x < 0f)
			{
				if (originalPosition.x < transform.position.x)
					rb2D.AddForce ( new Vector2 (-moveSpeed, 0f));
				else
					rb2D.AddForce ( new Vector2 (moveSpeed, 0f));
			}
			else
			{
				rb2D.AddForce ( new Vector2 (moveSpeed, 0f));
			}

			rb2D.velocity = new Vector2 (rb2D.velocity.x, 0f);
		}

	}
}
