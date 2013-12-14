using UnityEngine;
using System.Collections;

	[System.Serializable]
	public class MovementProperties		// These classes keep the Unity inspector more organized
	{
		public float moveSpeed = 1800f;							// Movement speed while grounded				
		public float airMoveSpeed = 500f;						// Movement speed while in the air
		[Range(1f,20f)] public float maxSpeedXGround = 8f;		// Max movement speed while grounded	
		[Range(1f,20f)] public float maxSpeedXAir = 12f;		// Max movement speed while in the air  
		[Range(1f,20f)] public float maxSpeedY = 15f;			// Max movement speed in the Y axis
		public float movingPlatformFriction = 7.7f;				// Experiment with this
		public float slopeClimbForce = 50f;						// How quickly you climb slopes
		[Range(0.5f,1f)] public float slopeLimit = 0.8f;	
	}

	[System.Serializable]
	public class JumpProperties
	{
		public Vector2 jumpForce =  new Vector2(0, 800);		//normal jump force
		public float jumpLeniancy = 0.17f;						//how early before hitting the ground you can press jump, and still have it work
		public float landingSlideAmount = 20f;
		public float landingSlideYForce = 20f;
		public float jumpInputStrength = 2f;
		public float angledJumpYAddedForce = 250f; 
		public float jumpCheckTime = 0.5f;
	}

	public class PlayerController : MonoBehaviour 
	{		

		public MovementProperties movementProps;
		public JumpProperties jumpProps;

		private Transform groundCasts;

		public LayerMask groundLayers;						// These layers will be included in the linecasts to test if grounded
		public float groundLinecastLength = 1f;

		private Animator animator;
		private AnimHashIDs animHashIDs;
		private Rigidbody2D rb2D;
		private int onJump;
		private float speedMode, hInput, jumpTime, slope;
		private bool grounded, groundedDelayCheck, groundedDelayCheckReset; 
		private bool isJumping, isJumpingDelayed, isOnMovingPlatform, hInputActivated;

		private bool isThisMyObject;
		private TNSyncPlayer tnSyncPlayer;
		void Awake ()
		{
			if (TNManager.isThisMyObject)
			{
				isThisMyObject = true;
				tnSyncPlayer = GetComponent<TNSyncPlayer>();
			}
		}
		void Start()
		{
			if (isThisMyObject)
			{
				rb2D = rigidbody2D;															// Cache rigidbody2D for performance reasons
		//		animator = transform.Find ("Boy").gameObject.GetComponent<Animator>();		// Get Boy's Animator to change animations
		//		animHashIDs = transform.Find("Boy").gameObject.GetComponent<AnimHashIDs>();
			}

		}

		void Update()
		{	
			if (isThisMyObject)
			{
				

				//get horizontal input
				hInput = Input.GetAxisRaw ("Horizontal");

				if (Mathf.Approximately(Mathf.Abs (hInput), 1f) && !hInputActivated)		// SlideCheck() Activates when HInputActivated is false
				{
					hInputActivated = true;
					CancelInvoke("ResetHInputActivated");
					Invoke("ResetHInputActivated", 0.2f);
				}
			}
		}

		bool crossFadingLanding;
		bool crossFadingFalling;
		void ResetCrossFadingLanding () { crossFadingLanding = false; }
		void ResetCrossFadingFalling () { crossFadingFalling = false; }

		void FixedUpdate() 
		{
			if (isThisMyObject)
			{

				JumpSetup ();

				speedMode = movementProps.moveSpeed;

				if (isJumpingDelayed)		// If we have just landed from a jump
				{
			
					if (Mathf.Abs (hInput) > 0.5f) // If the player is applying hInput, add force in the jump direction
					{
						rb2D.AddForce(new Vector2 (jumpProps.landingSlideAmount * hInput, jumpProps.landingSlideYForce));
						tnSyncPlayer.Sync();
					}
					// Reset jump bool variables
					isJumping = false;
					isJumpingDelayed = false;
				}

				groundedDelayCheck = true;			// groundedDelayCheck is used to allow a forgiving delayed jump


				float moveTo = transform.position.x + hInput;

				Move (moveTo, speedMode, 0f,  0.7f);	// Tested stopDistance, 0.7f is the best

				if (!isJumpingDelayed) 
					LimitSpeed (movementProps.maxSpeedXGround, movementProps.maxSpeedY);
				else 
					LimitSpeed (movementProps.maxSpeedXAir, movementProps.maxSpeedY);


			}
		}

		void ResetHInputActivated () { hInputActivated = false; }


		private void Move(float finishPosition, float speed, float tempSlope, float stopDistance)
		{
			float relativePosition = finishPosition - transform.position.x;
		     
		     // If we are traveling down the slope, set the slope for movement to 0
		     if (tempSlope < 0f && rb2D.velocity.x < 0f || tempSlope > 0f && rb2D.velocity.x > 0f) 
			 	tempSlope = 0f;
		     
		     float moveForceX = relativePosition * speed * Time.deltaTime;
		     
		     Vector2 moveForce = new Vector2 (moveForceX, Mathf.Abs(tempSlope * movementProps.slopeClimbForce));
		     
		     if (Mathf.Abs (relativePosition) > stopDistance)
			{
		     	rb2D.AddForce(moveForce);
				tnSyncPlayer.Sync();
			}
		}
		
		//jumping
		private void JumpSetup()
		{
			if (Input.GetButton ("Jump"))
				Jump (jumpProps.jumpForce);
		}
		
		//push player at jump force
		public void Jump(Vector3 jumpVelocity)
		{

			//animator.SetBool(animHashIDs.jumping,true);
		//	animator.Play (animHashIDs.jumpState);
		//	Invoke("ResetAnimJumpingBool", 0.05f);

			// Set the y velocity to zero so we have a reliable jump height every time
			rb2D.velocity = new Vector2 (rb2D.velocity.x, 0f);

			if (Mathf.Abs (hInput) > 0.1f) jumpVelocity.x = hInput * jumpProps.jumpInputStrength;	// If there's horizontal input (h) add it to jump force

			rb2D.AddForce(jumpVelocity);
	//		tnSyncPlayer.Sync();
			jumpTime = 0f;

		}

		//void ResetAnimJumpingBool () { animator.SetBool(animHashIDs.jumping,false); }


		void SlideCheck ()
		{
			if (!grounded || isJumping) return;

			if(Mathf.Abs(hInput) < 0.9f && slope < movementProps.slopeLimit)
			{
				rb2D.velocity = new Vector2 (rb2D.velocity.x * 0.5f, rb2D.velocity.y);
			}
		}

//		void RotateToGround ()
//		{
//			Quaternion fromRotation = transform.rotation;
//			Quaternion toRotation;
//			if (groundLinecast)
//			{
//				if (Mathf.Abs (slope) < movementProps.slopeLimit)
//				{
//					toRotation = Quaternion.FromToRotation(Vector2.up, groundLinecast.normal);
//					float weight = 0f;
//					if(weight <= 1f) 
//					{
//						weight += Time.deltaTime * 10f;
//						transform.rotation = Quaternion.Slerp(fromRotation, toRotation, weight);
//					}
//				}
//			}
//			else 
//			{
//				toRotation = Quaternion.identity;
//				float weight = 0f;
//				if(weight <= 1f) 
//				{
//					weight += Time.deltaTime * 2f;
//					transform.rotation = Quaternion.Slerp(fromRotation, toRotation, weight);
//				}
//			}
//		}

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