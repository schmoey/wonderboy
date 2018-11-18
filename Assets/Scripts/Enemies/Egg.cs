using UnityEngine;
using System.Collections;

public class Egg : Enemy {
	public string crackedSprite = "items/egg/2";
	public string puffSprite = "effects/puff/1";
	public GameObject spawnPrefab;
	public float walkKickStrength = 200f;
	public float runKickStrength = 100f;

	private BoxCollider2D collider;
	private Rigidbody2D rb;
	private bool isDead = false;
	private int health = 2;
	public bool isCracked = false;
	private tk2dSprite sprite;

	private bool isKicked = false;

	void Awake()
	{
		sprite = GetComponent<tk2dSprite> ();
		rb = GetComponent<Rigidbody2D> ();
		collider = GetComponent<BoxCollider2D> ();
	}

	void Start () {
	}
	
	void Update () {
			
	}

	public override void Hit ()
	{
		if (!isCracked) {
			isCracked = true;

			sprite.SetSprite (crackedSprite);
		}
		
		if (!isDead) {
			health--;

			if (health == 0) {
				isDead = true;

				Die ();
			}
		}
	}

	public void Kick(bool isRunning)
	{
		gameObject.layer = LayerMask.NameToLayer ("Dead");

		if (!isDead) {
			if (!isKicked) {
				sprite.SetSprite (crackedSprite);

				isKicked = true;

				collider.isTrigger = false;
				rb.isKinematic = false;
				rb.AddForce (new Vector2 (1f, 1f) * ((isRunning) ? runKickStrength : walkKickStrength));
			}
		}
	}

	public void OnCollisionEnter2D(Collision2D col)
	{
		Debug.Log (col.gameObject.layer);

		if (col.gameObject.layer == LayerMask.NameToLayer ("Ground TM")) {
			rb.Sleep ();
			Die ();
		}
	}

	IEnumerator SpawnItem()
	{
		yield return new WaitForSeconds (0.05f);

		GameObject.Instantiate (spawnPrefab, transform.position, Quaternion.identity);

//		Debug.Log (g.transform.localScale.x);

		yield return new WaitForSeconds (0.1f);

		Destroy (this.gameObject);

		yield return null;
	}

	void Die()
	{
		sprite.SetSprite (puffSprite);
		transform.localScale = new Vector3 (1.2f, 1.2f, 1f);
		transform.Translate (new Vector3(0f, 0.4f, 0));

		StartCoroutine (SpawnItem ());
	}

	public override void CloseActivate (){}
}
