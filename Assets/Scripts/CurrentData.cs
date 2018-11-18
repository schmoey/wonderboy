using UnityEngine;
using System.Collections;

public class CurrentData : MonoBehaviour {
	public string currentRespawnPointName;
	public int lives = 3;
	public int score = 0;

	public static CurrentData instance { get; private set; }

	void Awake()
	{
		if (instance == null) {
			DontDestroyOnLoad (this);
			instance = this;
		}
		else if (instance != this) {
			Destroy (gameObject);
		}
	}

	void Start () {
	
	}
	
	void Update () {
		if (Input.GetKeyDown (KeyCode.E)) {
			UnityEngine.SceneManagement.SceneManager.LoadScene (1);
		}
	}
}
