using UnityEngine;
using System.Collections;

public class ParticlePauseOnStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Invoke("PauseParticle", 1f);
	}
	
	void PauseParticle ()
	{
		particleSystem.Pause ();
	}
}
