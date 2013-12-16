using UnityEngine;
using System.Collections;

[System.Serializable]
public class MoveProps
{
	public float moveSpeed = 1800f;										
	public float maxSpeedX = 22f;		
	public float maxSpeedY = 22f;	
	public float jumpForceX = 2f;
	public float jumpForceY = 1000f;
}

public class PlayerController : MonoBehaviour 
{		

	public MoveProps moveProps;

	Animator animator;
	AnimHashIDs animHashIDs;
	Rigidbody2D rb2D;
	float hInput;
	bool isThisMyObject;

	void Awake ()
	{
		if (TNManager.isThisMyObject)
		{
			isThisMyObject = true;
		}
		GameManager.Instance.AddPlayersToDict();
		rb2D = rigidbody2D;	
	}

	void Update()
	{	
		if (isThisMyObject)
		{
			hInput = Input.GetAxisRaw ("Horizontal");
		}
	}

	void FixedUpdate() 
	{
		if (isThisMyObject)
		{
			JumpSetup ();

			float moveTo = transform.position.x + hInput;

			Move (moveTo, moveProps.moveSpeed, 0.7f);	


			LimitSpeed (moveProps.maxSpeedX, moveProps.maxSpeedY);

		}
	}

	private void Move(float finishPosition, float speed, float stopDistance)
	{
		float relativePosition = finishPosition - transform.position.x;

	    float moveForceX = relativePosition * speed * Time.deltaTime;
	     
		Vector2 moveForce = new Vector2 (moveForceX, 0f);

		if (Mathf.Abs (relativePosition) > stopDistance)
		{
			rb2D.AddForce(moveForce);
		}
	}
	
	private void JumpSetup()
	{
		if (Input.GetButton ("Jump"))
			Jump (moveProps.jumpForceY);
		if (Input.GetKey("s"))
			Jump (-moveProps.jumpForceY * 0.5f);

}

	public void Jump(float jumpForceY)
	{
//		rb2D.velocity = new Vector2 (rb2D.velocity.x, 0f);
//
		float xForce = 0f;

		if (Mathf.Abs (hInput) > 0.1f) xForce = hInput * moveProps.jumpForceX;

		Vector2 force = new Vector2 (xForce , jumpForceY);

		rb2D.AddForce (force);
	}


	public void LimitSpeed(float maxSpeedX, float maxSpeedY)
	{	
		bool limitX = false, limitY = false;
		Vector2 newVelocity = Vector2.zero;
		
		if (Mathf.Abs (rb2D.velocity.x) > maxSpeedX)
		{
			newVelocity = new Vector2(rb2D.velocity.normalized.x * maxSpeedX, rb2D.velocity.y);
			limitX = true;
		}
		if (Mathf.Abs (rb2D.velocity.y) > maxSpeedY)
		{
			newVelocity = new Vector2(rb2D.velocity.x, rb2D.velocity.normalized.y * maxSpeedY);
			limitY = true;
		}
		
		if (limitX || limitY)
		{
			rb2D.velocity = newVelocity;
		}
		
	}
}