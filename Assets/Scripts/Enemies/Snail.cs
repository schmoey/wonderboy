using UnityEngine;
using System.Collections;


public class Snail : Enemy {
	private BoxCollider2D collider;
	private Rigidbody2D rb;
	private int health = 1;
	private bool isDead = false;

	public float deathStrength = 10f;
	public float moveSpeed = 1f;

	void Awake()
	{
		animator = GetComponent<tk2dSpriteAnimator> ();
		collider = GetComponent<BoxCollider2D> ();
		rb = GetComponent<Rigidbody2D> ();
	}

	void Start () {

	}

	void Update () {
		base.Update ();

		if (isActive) {
			if (!isDead) {
				Move ();
			}
		}
	}

	void Move()
	{
		transform.Translate (Vector3.left * moveSpeed * Time.deltaTime);
	}

	void Die()
	{
		base.Die ();

		animator.Play ("Death");
		rb.isKinematic = false;
		rb.AddForce (new Vector2 (1, 1) * deathStrength);
		collider.size = Vector2.zero;
	}

	public override void Hit()
	{
		if (!isDead) {
			health--;

			if (health == 0) {
				isDead = true;

				Die ();
			}
		}
	}

	public override void CloseActivate(){}
}
