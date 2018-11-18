using UnityEngine;
using System.Collections;
using DG.Tweening;

public class TitleScreen : MonoBehaviour {
	public Transform logo;
	public Transform wipe;

	private RectTransform wipeTransform;

	void Awake()
	{
		wipeTransform = wipe.GetComponent<RectTransform> ();
	}

	void Start () {
		logo.DOLocalMoveY (10.4f, 6f).SetEase(Ease.Linear).From ();
	}

	public void StartGame()
	{
		wipeTransform.DOSizeDelta (new Vector2 (800f, 500f), 0.5f).SetEase (Ease.Linear).OnComplete (() => {
			UnityEngine.SceneManagement.SceneManager.LoadScene (1);	
		});
	}

	public void QuitGame()
	{
		Application.Quit ();
	}

	void _Update () {
		#if UNITY_EDITOR
		if (Input.anyKeyDown)
		{
			wipeTransform.DOSizeDelta (new Vector2 (800f, 500f), 0.5f).SetEase (Ease.Linear).OnComplete (() => {
				UnityEngine.SceneManagement.SceneManager.LoadScene (1);	
			});
		}
		#endif

		if (Input.touchCount > 0) {
			if (Input.touches[0].phase == TouchPhase.Began)
			{
				wipeTransform.DOSizeDelta (new Vector2 (800f, 500f), 0.5f).SetEase (Ease.Linear).OnComplete (() => {
					UnityEngine.SceneManagement.SceneManager.LoadScene (1);	
				});
			}
		}
	}
}
