using UnityEngine;
using System.Collections;
using DarkTonic.MasterAudio;

public class SoundManager : MonoBehaviour {
	public static SoundManager instance { get; private set; }

	// ugh

	void Awake()
	{
		if (instance == null) {
			instance = this;
		}
	}

	public void Axe()
	{
		MasterAudio.PlaySound ("axe");
	}

	public void Death()
	{
		MasterAudio.PlaySound ("death");
	}

	public void EnemyKill()
	{
		MasterAudio.PlaySound ("enemy_kill");
	}

	public void EnemyFairyKill()
	{
		MasterAudio.PlaySound ("enemy_fairy_kill");
	}

	public void Fruit()
	{
		MasterAudio.PlaySound ("fruit_get");
	}

	public void Item()
	{
		MasterAudio.PlaySound ("item_get");
	}

	public void Bump()
	{
		MasterAudio.PlaySound ("skateboard_lose");
	}

	public void Credit()
	{
		MasterAudio.PlaySound ("1P");
	}

	public void Jump()
	{
		MasterAudio.PlaySound ((Random.value >= 0.5f) ? "jump_1" : "jump_2");
	}

	public void Die()
	{

	}
}