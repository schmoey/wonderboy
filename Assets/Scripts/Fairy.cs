using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Fairy : MonoBehaviour {
	private Transform playerTransform;
	private DOTweenPath path;

	public static Fairy instance { get; private set; }

	void Awake() {
		if (instance == null) {
			instance = this;
		
			path = GetComponent<DOTweenPath> ();
		}
	}

	void Start () {
		playerTransform = Gameplay.instance.playerTransform;

		Spawn ();
	}

//	public void FlyAway()
//	{
//		transform.DOLocalMove (new Vector2 (5f, 15f), 1.5f).OnComplete(() => {
//			Destroy(this.gameObject);
//		});
//	}

	void Spawn()
	{
		Gameplay.instance.FairyPause (true);
		transform.SetParent (playerTransform);
		transform.localScale = Vector3.one;

		Vector3[] path = new Vector3[] { new Vector2 (-4f, 6f), new Vector2 (-3f, 3f), new Vector2 (-1f, 2f), new Vector2(-4f, 6f)};
	
		Sequence seq = DOTween.Sequence ();
		seq.Append (transform.DOLocalMove (new Vector2 (-4f, 6f), 0.5f));
		seq.Append (transform.DOLocalPath (path, 1.1f, PathType.CatmullRom, PathMode.TopDown2D, 10, Color.white).SetOptions(true).SetEase(Ease.Linear).SetLoops (0));

		seq.Append (transform.DOLocalMove (new Vector2 (-3f, 3f), 0.5f)).OnComplete (() => {
			Gameplay.instance.isFairy = true;
			Gameplay.instance.FairyPause(false);
		});;

		seq.Play ();
	}
}