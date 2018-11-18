using UnityEngine;
using System.Collections;

public class Food : MonoBehaviour {
	public GameObject scorePrefab;
	public GameObject scoreHigherPrefab;

	public string spriteName;
	public string higherSpriteName;

	public int scoreAmount;
	public int higherScoreAmount;

	public int healthAmount;
	public int higherHealthAmount;

	protected bool isHigher = false;
	private bool isActive = false;

	void Awake()
	{
		GetComponent<MeshRenderer> ().enabled = false;
	}

	void Start () {
		
	}
	
	void Update () {
		if (!isActive) {
			if (transform.position.x < Gameplay.instance.playerTransform.position.x + 10f) {
				GetComponent<MeshRenderer> ().enabled = true;
				isActive = true;
			}
		}
	}

	void Collect()
	{
		SoundManager.instance.Fruit ();

		gameObject.SetActive (false);
		GameObject.Instantiate (scorePrefab, transform.position, Quaternion.identity);
		Gameplay.instance.Score (scoreAmount);
		Gameplay.instance.Damage (healthAmount * -1);
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag ("Player")) {
			Collect ();
		}
	}
}