using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Snake : Enemy {
	private BoxCollider2D collider;
	private Rigidbody2D rb;
	private int health = 1;
	private bool isDead = false;
	public float nearRaiseHeight = 1.9f;
//	private bool isCloseActivation = false;

	public float deathStrength = 10f;

	void Awake()
	{
		isCloseActivation = true;

		transform.Translate (Vector3.down * nearRaiseHeight);

		animator = GetComponent<tk2dSpriteAnimator> ();
		collider = GetComponent<BoxCollider2D> ();
		rb = GetComponent<Rigidbody2D> ();
	}

	void Start () {
		
	}
	
	void Update () {
		base.Update ();
	}

	void Die()
	{
		base.Die ();

		animator.Play ("Death");
		rb.isKinematic = false;
		rb.AddForce (Vector2.up * deathStrength);
		collider.size = Vector2.zero;
	}

	public override void CloseActivate()
	{
		transform.DOMoveY (transform.position.y + nearRaiseHeight, 0.1f);
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
}
