using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	private float timer = 30.0f;
	private bool countingDown = true;
	private Camera mainCam;

	private Vector3 cancelTileCamPos = new Vector3(5f, 4.5f, -10f);
	private Vector3 battleResolveCamPos = new Vector3(5f, 20f, -10f);

	void Start () {
		mainCam = Camera.main;
		mainCam.transform.position = cancelTileCamPos;
	}

	void Update () {
		if (countingDown)
			timer -= Time.deltaTime;

		if (timer <= 0 && countingDown) {
			switchToBattleResolve();
		}
	}

	void OnGUI () {
		GUI.Box(new Rect(50, 50, 100, 90), "" + timer.ToString("0"));
	}

	private void switchToBattleResolve() {
		countingDown = true;
		mainCam.transform.position = battleResolveCamPos;
	}

	private void switchToCancelTiles() {
		countingDown = false;
		mainCam.transform.position = cancelTileCamPos;
	}
}
