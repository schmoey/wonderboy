using UnityEngine;
using System.Collections;

public class Boulder : Enemy {
	private BoxCollider2D collider;
	private Rigidbody2D rb;
	private int health = 100;
	private bool isDead = false;

	public float moveSpeed = 10f;


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
		transform.Translate (Vector2.left * moveSpeed * Time.deltaTime);
	}

	public override void Hit()
	{

	}

	public override void CloseActivate(){}
}
