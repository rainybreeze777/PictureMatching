using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	private float timer = 30.0f;
	private bool countingDown = true;
	private Camera mainCam;

	private Vector3 cancelTileCamPos = new Vector3(10.5f, 5f, -10f);
	private Vector3 battleResolveCamPos = new Vector3(10.5f, 20f, -10f);

	private BattleController battleController;
	private ComboController comboController;
	private BoardController boardController;

	void Awake () {
		battleController = GameObject.Find("BattleController").GetComponent<BattleController>() as BattleController;
		comboController = GameObject.Find("ComboController").GetComponent<ComboController>() as ComboController;
		boardController = GameObject.Find("BoardController").GetComponent<BoardController>() as BoardController;
	}

	void Start () {
		mainCam = Camera.main;
		mainCam.transform.position = cancelTileCamPos;
	}

	void Update () {
		if (countingDown)
			timer -= Time.deltaTime;

		if ((timer <= 0  || boardController.BoardIsEmpty()) && countingDown) {
			SwitchToBattleResolve();
		}
	}

	void OnGUI () {
		if (countingDown)
			GUI.Box(new Rect(50, 50, 100, 90), "" + timer.ToString("0"));
	}

	private void SwitchToBattleResolve() {
		countingDown = false;
		mainCam.transform.position = battleResolveCamPos;
		battleController.InitiateBattleResolution(comboController.GetCancelSeq());
	}

	private void SwitchToCancelTiles() {
		countingDown = true;
		mainCam.transform.position = cancelTileCamPos;
	}

	public void ChangeActiveState(string battleResult) {
		if (battleResult.Equals(BattleController.won)) {

		} else if (battleResult.Equals(BattleController.lost)) {

		} else if (battleResult.Equals(BattleController.unresolved)) {
			timer = 30.0f;
			boardController.ResetBoard();
			comboController.ClearCancelSequence();
			SwitchToCancelTiles();
		}
	}
}
