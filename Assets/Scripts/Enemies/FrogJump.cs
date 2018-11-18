using UnityEngine;
using System.Collections;

public class FrogJump : Enemy {
	private BoxCollider2D collider;
	private Rigidbody2D rb;
	private int health = 2;
	private bool isHit = false;
	private bool isDead = false;
	private bool isJumping = false;

//	public float nearJumpHeight = 1.9f;
	public float deathStrength = 10f;
	public float nearJumpStrength = 2000f;
	public bool isGrounded = true;

	public bool isJumpingFixable = true;

	void Awake()
	{
		isCloseActivation = true;

		animator = GetComponent<tk2dSpriteAnimator> ();
		collider = GetComponent<BoxCollider2D> ();
		rb = GetComponent<Rigidbody2D> ();
	}

	void Start () {
	}
	
	void Update () {
		base.Update();

		if (isGrounded && isJumpingFixable) {
			string currentIdleClip = "Idle";

			if (isHit) {
				currentIdleClip += "_hit";
			}

			if (!animator.IsPlaying (currentIdleClip)) {
				animator.Play (currentIdleClip);
			}
		}
	}

	IEnumerator JumpFix()
	{
		yield return new WaitForSeconds (0.1f);

		isJumpingFixable = true;

		yield return null;
	}

	void FixedUpdate()
	{
		Vector3 boundsMin = this.collider.bounds.min;

		isGrounded = Physics2D.OverlapArea (boundsMin, this.collider.bounds.max, LayerMask.GetMask ("Ground TM"));
	}

	public override void CloseActivate()
	{
		if (isGrounded) {
			isJumping = true;
			isJumpingFixable = false;

			animator.Play ("Leap");

			rb.isKinematic = false;
			rb.AddForce (new Vector2 (-1f, 3f) * nearJumpStrength);

			StartCoroutine (JumpFix ());
		}
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