using UnityEngine;
using System.Collections;

public class AnimHashIDs : MonoBehaviour {


	public int grounded;
	public int jumping;
	public int speed;

	public int fallState;
	public int jumpState;
	public int landState;

	void Awake ()
	{
		grounded = Animator.StringToHash("grounded");
		jumping = Animator.StringToHash("jumping");
		speed = Animator.StringToHash("speed");

		fallState = Animator.StringToHash("Base Layer.BoyFall");
		jumpState = Animator.StringToHash("Base Layer.BoyJump");
		landState = Animator.StringToHash("Base Layer.BoyLand");

	}
}
