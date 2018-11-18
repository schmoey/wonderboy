using UnityEngine;
using System.Collections;
using DG.Tweening;

public abstract class Enemy : MonoBehaviour {
	protected tk2dSpriteAnimator animator;
	protected bool isActive = false;
	protected bool isCloseActivation = false;
	protected bool isCloseActivated = false;
	public float closeActivationDistance = 0.5f;
	public float closeDetectionRadius = 5f;

	void Start () {
	}
	
	protected void Update () {
		if (!isActive) {
			if (transform.position.x < CameraManager.instance.transform.position.x + CameraManager.instance.camera.ScreenExtents.width) {
				isActive = true;
			}
		}

		if (isActive) {
			if (isCloseActivation) {
				if (!isCloseActivated) {
					RaycastHit2D hit = Physics2D.CircleCast (transform.position, closeDetectionRadius, Vector2.left, closeActivationDistance, LayerMask.GetMask ("Player"));

					if (hit) {
						isCloseActivated = true;
						CloseActivate ();
					}
				}
			}
		}
	}

	protected void Die ()
	{
		SoundManager.instance.EnemyKill ();
	}

	public abstract void CloseActivate ();

	public void Activate()
	{
//		isActive = true;
	}

	public abstract void Hit();
}
