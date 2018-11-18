using UnityEngine;
using System.Collections;

public class Bee : Enemy {
	private BoxCollider2D collider;
	private Rigidbody2D rb;
	private int health = 1;
	private bool isDead = false;
	private Vector3 originalPosition;

	public float moveSpeed = 10f;
	public float deathStrength = 200f;
	public float moveFrequency = 20f;
	public float moveMagnitutde = 0.5f;



	void Awake()
	{
		animator = GetComponent<tk2dSpriteAnimator> ();
		collider = GetComponent<BoxCollider2D> ();
		rb = GetComponent<Rigidbody2D> ();
		originalPosition = transform.position;
	}

	void Die()
	{
		base.Die ();

		animator.Play ("Death");
		rb.isKinematic = false;
		rb.AddForce (new Vector2(1f, 1f) * deathStrength);
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
//		transform.localPosition = Vector2.one * Mathf.Sin (Time.time) * moveMagnitutde;

		Vector3 v = transform.position;
		v.y = (Mathf.Sin(Time.time * moveFrequency) * moveMagnitutde) + originalPosition.y;
		transform.position = v + (Vector3.left * moveSpeed * Time.deltaTime);


//		transform.position = transform.up * Time.deltaTime * moveSpeed + Mathf.Sin (Time.deltaTime * moveFrequency) * moveMagnitutde;
//			transform.position = pos + axis * Mathf.Sin (Time.time * frequency) * magnitude;
		//transform.Translate (Vector2.left * moveSpeed * Time.deltaTime);
	}

	public override void CloseActivate (){}
}
