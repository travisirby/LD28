using UnityEngine;
using System.Collections;

public class SpriteFade : MonoBehaviour {

	public float flashTime;

	SpriteRenderer sprite;
	bool doneFlash, isReady, stopFlash, flashUpward;

	void OnEnable () 
	{
		sprite = gameObject.GetComponent<SpriteRenderer>();
		Invoke("FlashNow",1f);
	}

	void FlashNow ()
	{
		isReady = true;
		doneFlash = false;
		stopFlash = false;
		Invoke("StopFlash", flashTime);
	}

	void StopFlash ()
	{
		stopFlash = true;
	}

	
	void Update () 
	{
		if (doneFlash || !isReady) return;

		if (stopFlash && sprite.color.a >= 1.0f) doneFlash = true;

		Color col = sprite.color;

		if (!flashUpward && sprite.color.a > .2f)
		{
			col.a -= .04f;
			//Color.Lerp(sprite.color, col, time.DeltaTime();
			sprite.color = col;
		}
		else if (flashUpward && sprite.color.a < 1.0f)
		{
			col.a += .04f;
			sprite.color = col;
		}

		else if (flashUpward && sprite.color.a >= 1.0f)
		{
			flashUpward = false;
		}
		else if (!flashUpward && sprite.color.a <= 0.2f)
		{
			flashUpward = true;
		}
	}
}
