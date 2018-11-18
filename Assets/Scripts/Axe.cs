using UnityEngine;
using System.Collections;

public class Axe : MonoBehaviour {
	private Rigidbody2D rb;

	private float angularVelocity;
	private Vector2 velocity;

	public float throwForce = 10f;
	public float obstacleForce = 10f;

	public LayerMask obstacleLayer;
	public LayerMask enemyLayer;
	public LayerMask groundLayer;

	void Awake()
	{
		rb = GetComponent<Rigidbody2D> ();
	}

	void Start () {
		rb.isKinematic = false;
	}

	void OnEnable()
	{
		ResetPhysics ();
	}

	public void Throw()
	{
		rb.AddForce (Vector3.right * throwForce);
	}

	void ResetPhysics()
	{
		rb.isKinematic = true;
		rb.isKinematic = false;
	}

	void OnCollisionEnter2D (Collision2D col)
	{
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		// Enemy
		if (col.gameObject.layer == LayerMask.NameToLayer ("Enemy")) {
			col.GetComponent<Enemy> ().Hit ();

			rb.isKinematic = true;

			this.gameObject.SetActive(false);
		}

		if (((1 << col.gameObject.layer) & groundLayer) != 0) {
			rb.Sleep ();
			rb.gameObject.SetActive (false);
		}

		// Obstacle
		if (((1<<col.gameObject.layer) & obstacleLayer) != 0) {
			// Don't hit fire
			if (!col.gameObject.CompareTag ("Fire")) {
				rb.Sleep ();
				rb.AddForce (new Vector2 (-1f, 1f) * obstacleForce);
			}
		}

		if (col.gameObject.CompareTag ("Egg")) {
			Egg egg = col.GetComponent<Egg> ();

			if (!egg.isCracked) {
				rb.Sleep ();
				rb.AddForce (new Vector2 (-1.2f, 1.2f) * obstacleForce);
			}

			egg.Hit ();
		}
}
	
	void Update () {
		// Remove axe if it falls off screen
		if (transform.position.y < CameraManager.instance.camera.transform.position.y - CameraManager.instance.camera.ScreenExtents.height / 2) {
			gameObject.SetActive (false);
		}
	}
}