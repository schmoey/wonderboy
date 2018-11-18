using UnityEngine;
using System.Collections;


public class Puff : MonoBehaviour {
	private tk2dSpriteAnimator anim;

	void Awake(){
		anim = GetComponent<tk2dSpriteAnimator> ();
	}

	void Start () {
	
	}
	
	void Update () {
		if (!anim.IsPlaying ("Big")) {
			Destroy (this.gameObject);
		}
	}
}
