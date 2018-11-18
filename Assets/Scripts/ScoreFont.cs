using UnityEngine;
using System.Collections;
using TMPro;

public class ScoreFont : MonoBehaviour {
	public Material scoreMaterial;
	public Material scoreFairyMaterial;
	public TextMeshPro[] scoreCharacters;

	public bool isFairy = false;
	public float isAliveTime = 2f;
	private float timer;

	void Awake()
	{
		scoreCharacters = transform.GetComponentsInChildren<TextMeshPro> (); 
	}

	void Start () {
		
	}

	void Update()
	{
		timer += Time.deltaTime;

		if (timer > isAliveTime) {
			Destroy (this.gameObject);
		}
	}

	public void SetFairy(bool active)
	{
		for (int i = 0; i < scoreCharacters.Length; i++) {
			scoreCharacters [i].fontMaterial = (active) ? scoreFairyMaterial : scoreMaterial;
		}
	}
}
