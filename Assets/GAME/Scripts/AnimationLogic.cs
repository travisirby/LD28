using UnityEngine;
using System.Collections;

public class AnimationLogic : MonoBehaviour {

	public Animator animator;
	public string speed = "speed";

	private AnimHashIDs animHashIDs;
	private Rigidbody2D rb2D;

	void Awake () 
	{
		animHashIDs = transform.Find("Boy").gameObject.GetComponent<AnimHashIDs>();
		animator = transform.Find ("Boy").gameObject.GetComponent<Animator>();
		rb2D = rigidbody2D;
	}
	

	void FixedUpdate () 
	{

		if (rb2D.velocity.x < -1f && transform.localScale.x > 0f)
			transform.localScale = new Vector3 (-1f,1f,1f);
		if (rb2D.velocity.x > 1f && transform.localScale.x < 0f)
			transform.localScale = new Vector3 (1f,1f,1f);


		animator.SetFloat(animHashIDs.speed, Mathf.Abs(rb2D.velocity.x));
	}
}
