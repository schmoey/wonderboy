using UnityEngine;
using System.Collections;

public class FrogStill : Enemy {
	private BoxCollider2D collider;
	private Rigidbody2D rb;
	private int health = 2;
	private bool isHit = false;
	private bool isDead = false;

	public float nearJumpHeight = 1.9f;
	public float deathStrength = 10f;

	void Awake()
	{
		isCloseActivation = true;

		animator = GetComponent<tk2dSpriteAnimator> ();
		collider = GetComponent<BoxCollider2D> ();
		rb = GetComponent<Rigidbody2D> ();
	}

	public override void CloseActivate()
	{
	}

	void Die()
	{
		base.Die();

		animator.Play ("Death");
		rb.isKinematic = false;
		rb.AddForce (Vector2.up * deathStrength);
		collider.size = Vector2.zero;
	}

	public override void Hit()
	{
		if (!isDead) {
			health--;

			if (health == 1) {
				isHit = true;
			
				int currentFrame = animator.CurrentFrame;

				animator.Play(animator.CurrentClip.name + "_hit");
				animator.SetFrame (currentFrame);
			}

			if (health == 0) {
				isDead = true;

				Die ();
			}
		}
	}
}
