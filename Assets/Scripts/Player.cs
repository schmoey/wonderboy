using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public tk2dSpriteAnimator animator;
	public tk2dSprite sprite;
	private bool isRunning = false;
	private bool isJumping = false;
	private bool isStumbled = false;
	private bool isStumbleFixable = true;
	private bool isJumpingFixable = true;
	private bool hasWeapon = true;
	private bool isThrowing = false;
	public bool isGrounded = true;
	public bool isAlive = true;
	private Rigidbody2D rb;
	private BoxCollider2D col;
	private CircleCollider2D ccol;
	private int touchLeftId;
	private int touchRightId;
	private bool isWaitLeftTouch = false;
	private bool isWaitRightTouch = false;
	public float springStrength = 1000f;
	public float goalWalkSpeed = 1f;

	private ObjectPool objectPool;

	public bool isBoxCollider = true;
	public bool autoWalk = true;
	public float slopeFriction = 1f;
	public float walkSpeed = 1f;
	public float runSpeed = 2f;

	public GameObject puffPrefab;

	// jumpstrength value is 5100 for standing jump, which we're not using as he jumps high when walking in the original

	public float jumpStrength = 100f;
	public float superJumpStrength = 1000f;
	public float stumbleStrength = 400f;
	public Vector3 moveAmount = Vector3.zero;
	public GameObject axePrefab;
	public float deathForce = 500f;
	public LayerMask deathLayerMask;

	public float aliveMass = 5f;
	public float aliveGravityScale = 8f;
	public float deathMass = 3f;
	public float deathGravityScale = 6f;

	public bool toJump = false;
	public bool toRun = false;

	private float signOffsetX = 4.1f;

	void Awake()
	{
		sprite = GetComponent<tk2dSprite> ();
		animator = GetComponent<tk2dSpriteAnimator> ();
		rb = GetComponent<Rigidbody2D> ();
		col = GetComponent<BoxCollider2D> ();
		ccol = GetComponent<CircleCollider2D> ();

		transform.position = GameObject.Find(CurrentData.instance.currentRespawnPointName).transform.position + new Vector3(signOffsetX, 0, 0);
	}

	void Start () {
		// Weapon pool
		objectPool = GetComponent<ObjectPool> ();

		rb.gravityScale = aliveGravityScale;
		rb.mass = aliveMass;
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.collider.CompareTag ("Boulder")) {
			if (!Gameplay.instance.isFairy)
			{
				Die (true);
			}
			else
			{
				// FAIRY DEATH
				SoundManager.instance.EnemyFairyKill();

				GameObject.Instantiate (puffPrefab, col.transform.position, Quaternion.identity);
				Gameplay.instance.SpawnScore (col.transform.position, true);
				Destroy (col.gameObject);
			}
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (isAlive) {
			if (!Gameplay.instance.isFairy) {
				if (col.gameObject.layer == LayerMask.NameToLayer ("Obstacle")) {
					if (!isStumbled) {
						isStumbled = true;
						isStumbleFixable = false;
						rb.Sleep ();

						animator.Play ("Stumble");
						StartCoroutine (StumbleFix ());
						rb.AddForce (transform.up * stumbleStrength);

						SoundManager.instance.Bump ();

						Gameplay.instance.Damage (4);
					}
				}

				if (((1 << col.gameObject.layer) & deathLayerMask) != 0) {
					if (col.gameObject.CompareTag ("Fire")) {
						animator.Play ("DeathFire");
					} else {
						animator.Play ("Death");
					}

					Die ();
				}

				if (col.CompareTag ("Egg")) {
					col.GetComponent<Egg> ().Kick (isRunning);
				}
			} else {
				// FAIRY DEATH
				if ((col.gameObject.layer == LayerMask.NameToLayer ("Obstacle")) || (((1 << col.gameObject.layer) & deathLayerMask) != 0)) {
					SoundManager.instance.EnemyFairyKill ();

					GameObject.Instantiate (puffPrefab, col.transform.position, Quaternion.identity);
					Gameplay.instance.SpawnScore(col.transform.position, true);
					Destroy (col.gameObject);
				}
			}
				
			// Fairy agnostic
			if (col.CompareTag ("Spring")) {
				Spring ();
				col.GetComponent<tk2dSprite> ().SetSprite ("stage/spring/open");
			}
				
			if (col.CompareTag ("Sign")) {
				CurrentData.instance.currentRespawnPointName = col.gameObject.name;
			}

			if (col.CompareTag ("Goal")) {
				Gameplay.instance.Goal ();
			}
		}
	}

	public void Die(bool forceDeathAnimation = false, bool dieQuietly = false)
	{
		isAlive = false;

		SoundManager.instance.Death ();

		if (forceDeathAnimation) {
			animator.Play ("Death");
		}

		if (!dieQuietly) {
			moveAmount = Vector3.zero;

			col.enabled = false;
			rb.Sleep ();
			rb.gravityScale = deathGravityScale;
			rb.mass = deathMass;
			rb.AddForce (Vector2.up * deathForce);
		}

//		Gameplay.instance.RegisterHighScore ();

		StartCoroutine (Gameplay.instance.Reload ());
	}

	IEnumerator StumbleFix()
	{
		yield return new WaitForSeconds(0.1f);
		isStumbleFixable = true;
		yield return null;
	}

	IEnumerator JumpFix()
	{
		yield return new WaitForSeconds (0.1f);
		isJumpingFixable = true;
		yield return null;
	}

	void NormaliseSlope () {
		// Attempt vertical normalisation
		if (isGrounded) {
			RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 1f, LayerMask.NameToLayer("Ground"));

			if (hit.collider != null && Mathf.Abs(hit.normal.x) > 0.1f) {
				rb.velocity = new Vector2(rb.velocity.x - (hit.normal.x * slopeFriction), rb.velocity.y);

				Vector3 pos = transform.position;
				pos.y += -hit.normal.x * Mathf.Abs(rb.velocity.x) * Time.deltaTime * (rb.velocity.x - hit.normal.x > 0 ? 1 : -1);
				transform.position = pos;
			}
		}
	}

	public void SetPause(bool active)
	{
		rb.isKinematic = active;
	}

	void FixedUpdate()
	{
		if (isBoxCollider) {
			Vector3 boundsMin = col.bounds.min;

			if (Gameplay.instance.isSlope) {
				boundsMin.y -= 0.2f;
			}
				
			isGrounded = Physics2D.OverlapArea (boundsMin, col.bounds.max, LayerMask.GetMask ("Ground TM"));

			// Hurredly put in that the player must be coming down from the jump to hit platforms, but it was messing with moving platforms when going down
			// So 0.5 seems to make all the difference

			if (isGrounded && rb.velocity.y > 0.5f) {
				isGrounded = false;
			}
		} else {
			Vector3 boundsMin = ccol.bounds.min;

			if (Gameplay.instance.isSlope) {
				boundsMin.y -= 0.2f;
			}

			isGrounded = Physics2D.OverlapArea (boundsMin, ccol.bounds.max, LayerMask.GetMask ("Ground TM"));

			if (isGrounded && rb.velocity.y > 0) {
				isGrounded = false;
			}
		}

	}

	void SpawnAxe()
	{
		GameObject axe = objectPool.GetPooledObject ();

		if (axe == null) {
			return;
		}

		animator.Play ("Throw");
		SoundManager.instance.Axe ();

		axe.transform.localScale = new Vector3(0.4f, 0.4f, 1f);
		axe.transform.position = new Vector3 (transform.position.x - 0.8f, transform.position.y + 0.8f, transform.position.z);

		axe.SetActive (true);
		axe.GetComponent<Axe> ().Throw ();
	}

	void Weapon()
	{
		if (hasWeapon && !isThrowing) {
			isThrowing = true;

			SpawnAxe ();
		}
	}

	void Spring()
	{
		rb.Sleep ();
		rb.AddForce (transform.up * springStrength);
		isJumping = true;
		animator.Play ("Jump");
	}

	void Jump()
	{
		if (isGrounded) {
			if (!isJumping) {
				isJumping = true;
				isJumpingFixable = false;
				StartCoroutine (JumpFix ());

				float jumpStrengthVal = (isRunning) ? superJumpStrength : jumpStrength;
				rb.AddForce (transform.up * jumpStrengthVal);
			}

			SoundManager.instance.Jump ();
			animator.Play ("Jump");
		}
	}
	
	void Update () {
		if (Gameplay.instance.isFairyPause) {
			animator.Pause ();
		} else {
			if (animator.Paused) {
				animator.Resume ();
			}
		}


		// GOAL WALK
		if (Gameplay.instance.isGoal) {
			moveAmount = Vector3.right * goalWalkSpeed * Time.deltaTime;
			transform.Translate (moveAmount);
		}

		// All input code is hideous
		// TODO: switch to rewired

		// Touch screen input
		if (!Gameplay.instance.isFairyPause && isAlive && Gameplay.instance.isGameReady && !Gameplay.instance.isGoal) {
			if (Input.touchCount > 0 && Input.touchCount < 3) {
				for (int i = 0; i < Input.touchCount; i++) {
					
					// New finger
					if (Input.touches [i].phase == TouchPhase.Began) {
						// Screen left
						if (Input.touches [i].position.x < Screen.width / 2) {
							touchLeftId = Input.touches [i].fingerId;
							// Set wait one frame before hold detect
							isWaitLeftTouch = true;

							Weapon ();
						}

						// Screen right
						if (Input.touches [i].position.x > Screen.width / 2) {
							touchRightId = Input.touches [i].fingerId;
							// Set wait one frame before hold detect
							isWaitRightTouch = true;

							Jump ();
						}
					}

					TouchPhase tp = Input.touches [i].phase;

					if (tp == TouchPhase.Stationary || tp == TouchPhase.Moved) {
						if (Input.touches [i].position.x < Screen.width / 2) {
							toRun = true;	
						}

						if (Input.touches [i].position.x > Screen.width / 2) {
							toJump = true;
						}
					}
				}
			}
		
			if (isJumping && !isThrowing && !animator.IsPlaying("Jump")) {
				animator.Play ("Jump");
			}
				
		
			if (isAlive) {
				if (isThrowing) {
					if (!animator.IsPlaying ("Throw")) {
						isThrowing = false;
					}
				}

				float horizontalMovement = Input.GetAxisRaw ("Horizontal");

				if (isGrounded && isJumpingFixable) {
					isJumping = false;

					if (isStumbled) {
						if (isStumbleFixable) {
							isStumbled = false;
						}
					}
				}

				if (toRun) {
					isRunning = true;
					toRun = false;
				} else {
					isRunning = false;
				}


				if (!isStumbled) {
					#if UNITY_EDITOR
					if (Input.GetKey (KeyCode.Z)) {
						isRunning = true;
					} else {
						isRunning = false;
					}

					if (Input.GetKeyDown (KeyCode.Z)) {
						Weapon ();
					}


					if (Input.GetKey (KeyCode.X)) {
						toJump = true;
					} else {
						toJump = false;
					}
					#endif


					if (toJump) {
						Jump ();
						toJump = false;
					}
				}
		

				// AUTO WALK
				if (autoWalk) {
					string animationClip = (isRunning) ? "Run" : "Walk";

					float movementSpeed = (isRunning) ? runSpeed : walkSpeed;

					if (!isStumbled && !isJumping && !isThrowing) {
						animator.Play (animationClip);
					}

					moveAmount = Vector3.right * movementSpeed * Time.deltaTime;
					transform.Translate (moveAmount);
				} else {
					// MANUAL WALK
					if (Mathf.Abs (horizontalMovement) > 0.1f) {
						string animationClip = (isRunning) ? "Run" : "Walk";

						float movementSpeed = (isRunning) ? runSpeed : walkSpeed;

						if (horizontalMovement < 0) {
							movementSpeed *= -1;
							sprite.FlipX = true;
						}

						if (horizontalMovement > 0) {
							sprite.FlipX = false;
						}

						if (!isStumbled && !isJumping && !isThrowing) {
							animator.Play (animationClip);
						}

						moveAmount = Vector3.right * movementSpeed * Time.deltaTime;
						transform.Translate (moveAmount);
					}
				}
			}
		}
	}
}