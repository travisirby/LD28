using UnityEngine;
using System.Collections;
using TNet;

public class GameManager : Singleton<GameManager> {

	protected GameManager () {} // guarantee this will be always a singleton only - can't use the constructor!

	[RCC(255)]
	static GameObject OnCreate (GameObject prefab, Vector3 pos, Quaternion rot, Vector2 moveToPos)
	{
		Debug.Log ("created");
		GameObject go = Instantiate(prefab, pos, rot) as GameObject;
		go.SendMessage("MoveToPos", moveToPos, SendMessageOptions.DontRequireReceiver);
		return go;
	}

	public void CreateHarpoon (GameObject prefab, Vector3 pos, Quaternion rot, Vector2 moveToPos, bool persistent = false)
	{
		TNManager.CreateEx(255, persistent, prefab, pos, rot, moveToPos);
		Debug.Log ("created");
	}
}
