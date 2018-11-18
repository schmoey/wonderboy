using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class Gameplay : MonoBehaviour {
	public static Gameplay instance { get; private set; }

	private Transform cameraTransform;
	private Player player;
	private tk2dRuntime.TileMap.TileInfo tileInfo;
	private int tileX;
	private int tileY;
	private float playerXCameraOffset;

	public int lives;
	private int score;
	public bool isFairy = false;
	public Transform currentRespawnPoint;

	public bool isGoal = false;
	private bool isGoalWalkStarted = false;
	private bool hasSloped = false;
	public bool isSlope = false;

	public bool isGameReady = false;
	private float tempGroundPosition;

	private bool isCameraLerping = false;
	public float acceptableCameraRange = 0.1f;
	private bool isFirstFrame = true;

	public tk2dTileMap tilemap;
	public tk2dCamera camera;
	public Transform playerTransform;
	public float enemyActivationRange = 10f;

	public GameObject vitalityGroup;
	public GameObject[] vitality;
	public RectTransform vitalityMask;
	public TextMeshProUGUI highScoreText;
	public TextMeshProUGUI gameOverText;

	private int highScore;

	public int health = 32;
	public int healthMax = 32;
	public float healthTimerAmount = 2f;
	public TextMeshProUGUI scoreText;
	public int scoreAmountPerSecond = 10;

	private float timer = 0f;
	private float healthTimer = 2f;

	private RaycastHit2D[] enemiesToActivate;
	private bool resetCameraGround = false;
	private bool isGoingUp = false;
	public TextMeshProUGUI openingText;

	public bool isFairyPause = false;

	public GameObject[] scorePrefabs;
	public Dictionary<int, GameObject> scorePrefabsDict;

	public float fairyTime = 10f;
	private float fairyTimer = 0f;
	private int fairyScoreCount;
	private int fairyScoreMax = 22;

	public float currentGroundHeight = -2f;
	private float lastGroundPositionY = -2f;

	public GameObject livesPanel;
	public GameObject lifeImage;

	private int[] fairyScores = new int[] {
		50,
		50,
		50,
		50,
		100,
		100,
		100,
		200,
		200,
		200,
		500,
		500,
		1000,
		1000,
		2000,
		2000,
		3000,
		3000,
		5000,
		5000,
		10000
	};

	void Awake()
	{
		if (instance == null) {
			instance = this;
		}

		// Load score
		if (PlayerPrefs.HasKey ("HighScore")) {
			highScore = PlayerPrefs.GetInt ("HighScore");
			highScoreText.text = highScore.ToString();
		} else {
			highScoreText.text = "0";
		}

		// Set lives
		lives = CurrentData.instance.lives;

		for (int i = 0; i < lives; i++) {
			GameObject.Instantiate (lifeImage, livesPanel.transform);
		}

		// Set score
		score = CurrentData.instance.score;
		scoreText.text = score.ToString ();

		scorePrefabsDict = new Dictionary<int, GameObject> ();

		scorePrefabsDict.Add (50, scorePrefabs [0]);
		scorePrefabsDict.Add (100, scorePrefabs [1]);
		scorePrefabsDict.Add (200, scorePrefabs [2]);
		scorePrefabsDict.Add (500, scorePrefabs [3]);
		scorePrefabsDict.Add (1000, scorePrefabs [4]);
		scorePrefabsDict.Add (2000, scorePrefabs [5]);
		scorePrefabsDict.Add (3000, scorePrefabs [6]);
		scorePrefabsDict.Add (5000, scorePrefabs [7]);
		scorePrefabsDict.Add (10000, scorePrefabs [8]);

		cameraTransform = camera.transform;
		player = playerTransform.GetComponent<Player> ();

//		tilemap = GameObject.Find ("/Stage").GetComponent<tk2dTileMap>();
//		vitality = new GameObject[32];

		//		for (int i = 0; i < vitalityGroup.transform.childCount; i++) {
//			vitality [i] = vitalityGroup.transform.GetChild (i).gameObject;
//		}
	}

	private string ReverseString(string s)
	{
		char[] array = s.ToCharArray ();
		System.Array.Reverse (array);
		return new string (array);
	}

	public IEnumerator Start () {
		string[] texts = {
			"Try not to die",
			"Jump over stuff",
			"Time to run",
			"Snails are poisonous",
			"Eat food to survive"
		};
			
		openingText.text = texts[Random.Range(0, texts.Length)];

		playerXCameraOffset = cameraTransform.position.x - playerTransform.position.x;

		yield return new WaitForSeconds (0.5f);
		openingText.enabled = true;

		yield return new WaitForSeconds (1.5f);
		openingText.enabled = false;
		isGameReady = true;

		yield return null;
	}

	public void Quit()
	{
		Application.Quit ();
	}

	// Shitty local score
	// TODO: write to a server / gamecenter / google play
	public void RegisterHighScore()
	{
		PlayerPrefs.SetInt ("HighScore", score);
	}

	public void Damage(int amount)
	{
		health -= amount;

		if (health <= 0) {
			player.Die (true);
		}

		if (health >= healthMax) {
			health = healthMax;
		}
			
		Vector2 v = new Vector2((6f  * (healthMax - health) + (1.7f * (healthMax - health))), vitalityMask.sizeDelta.y);
		vitalityMask.sizeDelta = v;

//		// REDO THIS
//		for (int i = 1; i < health; i++) {
//			Color c = vitality [i - 1].GetComponent<Image> ().color;
//			c.a = 1f;
//
//			vitality [i - 1].GetComponent<Image> ().color = c;
//		}
//			
//		for (int i = health; i <= healthMax; i++) {
//			Color c = vitality [i - 1].GetComponent<Image> ().color;
//			c.a = 0.6f;
//
//			vitality [i - 1].GetComponent<Image> ().color = c;
//		}
	}

	void UpdateScore()
	{
		timer += Time.deltaTime;

		if (timer >= 1f) {
			timer = 0;

			Score (scoreAmountPerSecond);
		}
	}

	public void Score(int amount)
	{
		score += amount;

		if (score <= 0) {
			score = 0;
		}

		scoreText.text = score.ToString ();

		if (score > highScore) {
			highScore = score;
			highScoreText.text = highScore.ToString ();
		}
	}

	public void FairyPause(bool active)
	{
		isFairyPause = active;
		player.GetComponent<Player> ().SetPause (active);


		// Coming out of fairy pause, enter fairy invincibility state
		if (!active) {
			isFairy = true;
			fairyTimer = 0f;
		}
	}

	public void SpawnScore(Vector3 pos, bool isFairyText = false)
	{
		if (isFairyText)
		{
			fairyScoreCount++;
			GameObject fs = (GameObject)GameObject.Instantiate (scorePrefabsDict[fairyScores[fairyScoreCount]], pos, Quaternion.identity);
			fs.GetComponent<ScoreFont> ().SetFairy (true);

			Gameplay.instance.Score (fairyScores[fairyScoreCount]);
		}
	}

	void Update () {
		if (isGoal) {
			if (!isGoalWalkStarted) {
				isGoalWalkStarted = true;

				player.animator.Play ("Walk");
				player.animator.ClipFps = 2f;

				player.sprite.scale = new Vector3 (player.sprite.scale.x * -1f, player.sprite.scale.y, player.sprite.scale.z);
			}

		}

		// Attempt to get ground position
		if (player.isGrounded) {
			tempGroundPosition = lastGroundPositionY;
			lastGroundPositionY = player.transform.position.y;
		}

		if (!isFairyPause && player.isAlive && isGameReady) {

			// Falling death
			if (playerTransform.position.y < cameraTransform.position.y - camera.ScreenExtents.height / 2f) {
				player.Die (false, true);
			}

			if (player.isAlive) {
				UpdateScore ();

				healthTimer -= Time.deltaTime;

				if (healthTimer < 0) {
					healthTimer = healthTimerAmount;

					Damage (1);
				}

				// FAIRY INVINCIBILITY COUNTDOWN
				if (isFairy) {
					fairyTimer += Time.deltaTime;

					if (fairyTimer >= fairyTime) {
						Fairy.instance.transform.DOLocalMove (new Vector2 (8f, 18f), 2.5f).SetEase(Ease.Linear).OnComplete(() => {
							isFairy = false;
							Destroy(Fairy.instance.gameObject);
						});

//						Fairy.instance.FlyAway ();
					}
				}
			}

			if (Input.GetKeyDown (KeyCode.Escape)) {
				Application.Quit ();
			}

			if (Input.GetKeyDown (KeyCode.R)) {
				UnityEngine.SceneManagement.SceneManager.LoadScene (0);
			}

			RaycastHit2D hit = Physics2D.Raycast (playerTransform.position, Vector2.down, 200f, LayerMask.GetMask ("Ground TM"));

			if (hit) {
				tileInfo = tilemap.GetTileInfoForTileId (tilemap.GetTileIdAtPosition (hit.point, 1));

				if (tileInfo != null) {
					if (tileInfo.intVal == 1) {
						isSlope = true;

						tilemap.GetTileAtPosition (hit.point, out tileX, out tileY);
					} else {
						isSlope = false;
					}
				} else {
					isSlope = false;
				}

				int tileId = tilemap.GetTileIdAtPosition (hit.point, 1);

				tileInfo = tilemap.GetTileInfoForTileId (tilemap.GetTileIdAtPosition (hit.point, 1));
			}
		}
	}

	void LateUpdate()
	{
		// TODO: Better camera code
		if (!isFairyPause && player.isAlive && isGameReady && !isGoal) {
			cameraTransform.Translate (player.moveAmount);

			// Move camera vertically ON SLOPE
			if (tileInfo != null) {	
				if (tileInfo.intVal == 1) {
					hasSloped = true;

					Vector3 tilePosition = tilemap.GetTilePosition (tileX, tileY);
					float moveBy = lastGroundPositionY;

					// Going down?
					if (lastGroundPositionY < tempGroundPosition) {
						moveBy -= 10f;
					} else {
						moveBy += 10f;
					}

					cameraTransform.position = Vector3.Slerp (cameraTransform.position, new Vector3 (playerTransform.position.x + playerXCameraOffset - 1f, moveBy, cameraTransform.position.z), Time.deltaTime / 4);
				} else {
					if (isCameraLerping) {
						// TODO: Deal with down slopes
						cameraTransform.position = Vector3.Slerp (cameraTransform.position, new Vector3 (playerTransform.position.x + playerXCameraOffset - 1f, lastGroundPositionY + 2.45f, cameraTransform.position.z), Time.deltaTime);

						if (Mathf.Abs (cameraTransform.position.y - (lastGroundPositionY + 2.45f)) <= acceptableCameraRange) {
							isCameraLerping = false;
							hasSloped = false;
						}
					}

					if (hasSloped) {
						if (!isCameraLerping && (Mathf.Abs (cameraTransform.position.y - (lastGroundPositionY + 2.45f)) > acceptableCameraRange)) {
							isCameraLerping = true;
						}
					}
				}
			}
		}
	}

	public void Goal()
	{
		isGoal = true;
	}

	public IEnumerator Reload()
	{
		lives--;

		Destroy (livesPanel.transform.GetChild (0).gameObject);

		if (lives < 0) {
//			GameOver ();
			yield return new WaitForSeconds(0.5f);
			gameOverText.enabled = true;
			yield return new WaitForSeconds (2f);
			CameraManager.instance.WipeTransition (0);
		} else {
			// Save information to carry over during game, not round
			CurrentData.instance.lives = lives;
			CurrentData.instance.score = score;

			yield return new WaitForSeconds (2f);
			CameraManager.instance.WipeTransition (UnityEngine.SceneManagement.SceneManager.GetActiveScene ().buildIndex);
		}
	}

	private void GameOver()
	{
	}
}
