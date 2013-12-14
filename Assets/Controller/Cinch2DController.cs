using UnityEngine;
using System.Collections;
using Cinch2D;

namespace Cinch2D
{
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

	public class Cinch2DController : MonoBehaviour 
	{		

		public MovementProperties movementProps;
		public JumpProperties jumpProps;

		private Transform groundCasts;

		public LayerMask groundLayers;						// These layers will be included in the linecasts to test if grounded
		public float groundLinecastLength = 1f;

		private Transform[] groundCastChilds;
		private Animator animator;
		private AnimHashIDs animHashIDs;
		private RaycastHit2D groundLinecast;
		private Rigidbody2D rb2D;
		private int onJump;
		private float speedMode, hInput, jumpTime, slope;
		private bool grounded, groundedDelayCheck, groundedDelayCheckReset; 
		private bool isJumping, isJumpingDelayed, isOnMovingPlatform, hInputActivated;

		void Start()
		{
			rb2D = rigidbody2D;															// Cache rigidbody2D for performance reasons

	//		animator = transform.Find ("Boy").gameObject.GetComponent<Animator>();		// Get Boy's Animator to change animations
	//		animHashIDs = transform.Find("Boy").gameObject.GetComponent<AnimHashIDs>();

			groundCasts = transform.Find ("GroundCasts");								// Find our groundCasts child gameobject

			groundCastChilds = new Transform[groundCasts.childCount];	// Create a Transform array and find all groundCasts children 
			for (int i=0; i < groundCastChilds.Length; i++)				// Used for linecasting to see if we are grounded
				groundCastChilds[i] = groundCasts.GetChild(i);
		}

		void Update()
		{	
			JumpSetup ();

			//get horizontal input
			hInput = Input.GetAxisRaw ("Horizontal");

			if (Mathf.Approximately(Mathf.Abs (hInput), 1f) && !hInputActivated)		// SlideCheck() Activates when HInputActivated is false
			{
				hInputActivated = true;
				CancelInvoke("ResetHInputActivated");
				Invoke("ResetHInputActivated", 0.2f);
			}
		}

		bool crossFadingLanding;
		bool crossFadingFalling;
		void ResetCrossFadingLanding () { crossFadingLanding = false; }
		void ResetCrossFadingFalling () { crossFadingFalling = false; }

		void FixedUpdate() 
		{
			// grounded test
			grounded = CheckIfGrounded ();
			//AnimatorStateInfo currentAnimState = animator.GetCurrentAnimatorStateInfo(0);

			if (grounded)
			{
				//animator.SetBool(animHashIDs.grounded, true);
//				if (currentAnimState.nameHash == animHashIDs.fallState && !crossFadingLanding)
//				{
//					CancelInvoke ("ResetCrossFadingLanding");
//					crossFadingLanding = true;
//					//animator.CrossFade (animHashIDs.landState,0.05f,0,Time.deltaTime);
//					Invoke ("ResetCrossFadingLanding",0.15f);
//				}
				speedMode = movementProps.moveSpeed;

				if (isJumpingDelayed)		// If we have just landed from a jump
				{
		
					if (Mathf.Abs (hInput) > 0.5f)		// If the player is applying hInput, add force in the jump direction
						rb2D.AddForce(new Vector2 (jumpProps.landingSlideAmount * hInput, groundLinecast.normal.x * jumpProps.landingSlideYForce));

					// Reset jump bool variables
					isJumping = false;
					isJumpingDelayed = false;
				}

				groundedDelayCheck = true;			// groundedDelayCheck is used to allow a forgiving delayed jump
			}
			else // not grounded
			{
//				if (currentAnimState.nameHash != animHashIDs.fallState && !isJumping && !crossFadingFalling)
//				{
//					CancelInvoke ("ResetCrossFadingFalling");
//					crossFadingFalling = true;
//					//animator.CrossFade (animHashIDs.fallState,0.1f,0,Time.deltaTime);
//					Invoke ("ResetCrossFadingFalling",0.15f);
//				}
				//animator.SetBool(animHashIDs.grounded, false);
				speedMode = movementProps.airMoveSpeed;
				slope = 0f;

				if (groundedDelayCheck && !groundedDelayCheckReset)			// Reset groundedDelayCheck (used for forgiving jumps)
				{
					groundedDelayCheckReset = true;
					Invoke("GroundedDelayCheckReset", jumpProps.jumpCheckTime);	
				}
			}

			// Moving platform support
			Vector2 movingPlatformSpeed = Vector2.zero;
			if (grounded)
			{
				if (groundLinecast.transform.CompareTag("MovingPlatform"))
				{
					isOnMovingPlatform = true;
					movingPlatformSpeed = groundLinecast.transform.rigidbody2D.velocity;
					Vector2 newVel = new Vector2(movingPlatformSpeed.x * movementProps.movingPlatformFriction * Time.deltaTime, 0f);
					rb2D.velocity = new Vector2 (newVel.x, rb2D.velocity.y);

				}
				else
					isOnMovingPlatform = false;

			}


			float moveTo = transform.position.x + hInput;

			Move (moveTo, speedMode, slope,  0.7f);	// Tested stopDistance, 0.7f is the best

			if (!isJumpingDelayed) 
				LimitSpeed (movementProps.maxSpeedXGround + movingPlatformSpeed.magnitude, movementProps.maxSpeedY);
			else 
				LimitSpeed (movementProps.maxSpeedXAir + movingPlatformSpeed.magnitude, movementProps.maxSpeedY);

			if (!hInputActivated) SlideCheck();

			RotateToGround();	
		}

		void ResetHInputActivated () { hInputActivated = false; }

		void GroundedDelayCheckReset ()			// Called to reset groundedDelayCheck after we have been not grounded 
		{										// for jumpProps.jumpCheckTime (used so you can jump a little bit after you are not grounded)
			groundedDelayCheck = false;
			groundedDelayCheckReset = false;
		}

		private bool CheckIfGrounded() 		// Returns true if groundLineCast hits a collider2D which is in the groundLayers layermask.
		{
			// Check if we are grounded by linecasting from each groundCast position
			for (int i = 0; i < groundCastChilds.Length; i++)
			{
				Vector3 linecastStopPos = new Vector3 (groundCastChilds[i].localPosition.x, groundCastChilds[i].localPosition.y - groundLinecastLength, 0f);
				groundLinecast = Physics2D.Linecast(groundCastChilds[i].position, transform.TransformPoint(linecastStopPos), groundLayers);

				Debug.DrawLine(groundCastChilds[i].position, transform.TransformPoint(linecastStopPos), Color.red,2f);

				if (groundLinecast)
				{
					//find the slope of the collider we are on
					slope = groundLinecast.normal.normalized.x;
					// yes grounded
					return true;
				}
			}
			// not grounded
			return false;
		}

		private void Move(float finishPosition, float speed, float tempSlope, float stopDistance)
		{
			float relativePosition = finishPosition - transform.position.x;
		     
		     // If we are traveling down the slope, set the slope for movement to 0
		     if (tempSlope < 0f && rb2D.velocity.x < 0f || tempSlope > 0f && rb2D.velocity.x > 0f) 
			 	tempSlope = 0f;
		     
		     float moveForceX = relativePosition * speed * Time.deltaTime;
		     
		     Vector2 moveForce = new Vector2 (moveForceX, Mathf.Abs(tempSlope * movementProps.slopeClimbForce));
		     
		     if (Mathf.Abs (relativePosition) > stopDistance)
		     	rb2D.AddForce(moveForce);
		}
		
		//jumping
		private void JumpSetup()
		{
			//if we press jump in the air, save the time
			if (Input.GetButtonDown ("Jump") && !grounded)
				jumpTime = Time.time;
			
			//if were on ground within slope limit
			if (grounded || groundedDelayCheck)
			{
				if (Mathf.Abs (slope) < movementProps.slopeLimit)
				{
					//and we press jump, or we pressed jump justt before hitting the ground
					if (Input.GetButtonDown ("Jump") || jumpTime + jumpProps.jumpLeniancy > Time.time)
					{	
						groundedDelayCheck = false;
						groundedDelayCheckReset = false;
						Jump (jumpProps.jumpForce);
					}
				}
			}
		}
		
		//push player at jump force
		public void Jump(Vector3 jumpVelocity)
		{
			isJumping = true;

			//animator.SetBool(animHashIDs.jumping,true);
		//	animator.Play (animHashIDs.jumpState);
		//	Invoke("ResetAnimJumpingBool", 0.05f);

			// Set the y velocity to zero so we have a reliable jump height every time
			rb2D.velocity = new Vector2 (rb2D.velocity.x, 0f);

			if (Mathf.Abs (hInput) > 0.1f) jumpVelocity.x = hInput * jumpProps.jumpInputStrength;	// If there's horizontal input (h) add it to jump force

			if (groundLinecast.normal.x > 0 && rb2D.velocity.x < 0 || groundLinecast.normal.x < 0 && rb2D.velocity.x > 0)
			{
				jumpVelocity.y = jumpVelocity.y + Mathf.Abs (groundLinecast.normal.x) * jumpProps.angledJumpYAddedForce;
			}

			rb2D.AddForce(jumpVelocity);
			jumpTime = 0f;
			Invoke("SetIsJumpingDelayed", 0.2f);
		}

		//void ResetAnimJumpingBool () { animator.SetBool(animHashIDs.jumping,false); }

		void SetIsJumpingDelayed () { isJumpingDelayed = true; }

		void SlideCheck ()
		{
			if (!grounded || isJumping) return;

			if(Mathf.Abs(hInput) < 0.9f && slope < movementProps.slopeLimit)
			{
				rb2D.velocity = new Vector2 (rb2D.velocity.x * 0.5f, rb2D.velocity.y);
			}
		}

		void RotateToGround ()
		{
			Quaternion fromRotation = transform.rotation;
			Quaternion toRotation;
			if (groundLinecast)
			{
				if (Mathf.Abs (slope) < movementProps.slopeLimit)
				{
					toRotation = Quaternion.FromToRotation(Vector2.up, groundLinecast.normal);
					float weight = 0f;
					if(weight <= 1f) 
					{
						weight += Time.deltaTime * 10f;
						transform.rotation = Quaternion.Slerp(fromRotation, toRotation, weight);
					}
				}
			}
			else 
			{
				toRotation = Quaternion.identity;
				float weight = 0f;
				if(weight <= 1f) 
				{
					weight += Time.deltaTime * 2f;
					transform.rotation = Quaternion.Slerp(fromRotation, toRotation, weight);
				}
			}
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
}