using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour {

	private float timer = 30.0f;

	void Update () {
		timer -= Time.deltaTime;
	}

	void OnGUI () {
		GUI.Box(new Rect(50, 50, 100, 90), "" + timer.ToString("0"));
	}
}
