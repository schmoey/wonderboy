using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CameraManager : MonoBehaviour {
	public static CameraManager instance { get; private set; }
	public tk2dCamera camera;
	public Vector3 startPosition;
	public bool isDebug = false;
	public RectTransform wipeTransform;
	private float signOffsetX = 9.3f;

	void Start () {
	}

	void Awake()
	{
		if (instance == null) {
			instance = this;
		
			camera = GetComponent<tk2dCamera> ();
		}

		if (!isDebug) {
			Vector3 startPosition = GameObject.Find (CurrentData.instance.currentRespawnPointName).transform.position;
			startPosition.x += 10f;
			startPosition.y += 3.08f;
			startPosition.z = -10f;

			transform.position = startPosition;
		} else {
			transform.position = Gameplay.instance.currentRespawnPoint.position + new Vector3(signOffsetX, 0, 0);
		}

		wipeTransform.sizeDelta = new Vector2 (800f, 500f);

		wipeTransform.DOSizeDelta (new Vector2 (0, 500f), 0.5f).SetEase (Ease.Linear).OnComplete (() => {
//			Gameplay.instance.isGameReady = true;
		});
	}

	public void WipeTransition(int sceneIndex)
	{
		wipeTransform.DOSizeDelta (new Vector2 (800f, 500f), 0.5f).SetEase (Ease.Linear).OnComplete (() => {
			UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
		});
	}
}
