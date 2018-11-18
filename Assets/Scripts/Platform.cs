using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Platform : MonoBehaviour {
	private Transform transform;

	void Awake()
	{
		transform = GetComponent<Transform> ();
	}

	void Start () {
		transform.DOLocalMoveY (12f, 2f).SetEase(Ease.InOutCubic).SetLoops (-1, LoopType.Yoyo);
	}
	
	void Update () {
	
	}
}
