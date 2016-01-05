using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour {

    private float timer = 30.0f;
    private bool countingDown = false;
    private Camera mainCam;

    private Vector3 cancelTileCamPos = new Vector3(10.5f, 5f, -10f);
    private Vector3 battleResolveCamPos = new Vector3(10.5f, 20f, -10f);
    private Vector3 opEdCamPos = new Vector3(10.5f, -10f, -10f);

    private BattleController battleController;
    private ComboController comboController;
    private BoardController boardController;

    private Text playerHealthText;
    private Text enemyHealthText;
    private Text titleText;
    private GameObject startGameButton;
    private GameObject quitButton;

    void Awake () {
        battleController = GameObject.Find("BattleController").GetComponent<BattleController>() as BattleController;
        comboController = GameObject.Find("ComboController").GetComponent<ComboController>() as ComboController;
        boardController = GameObject.Find("BoardController").GetComponent<BoardController>() as BoardController;
    
        playerHealthText = GameObject.Find("PlayerHealth").GetComponent<Text>();
        enemyHealthText = GameObject.Find("EnemyHealth").GetComponent<Text>();
        titleText = GameObject.Find("TitleText").GetComponent<Text>();
        startGameButton = GameObject.Find("StartGameButton");
        quitButton = GameObject.Find("QuitButton");
    }

    void Start () {
        mainCam = Camera.main;
        mainCam.transform.position = opEdCamPos;

        titleText.text = "Picture Matching";
        startGameButton.GetComponent<Button>().GetComponentInChildren<Text>().text = "Start Game";
        playerHealthText.enabled = false;
        enemyHealthText.enabled = false;

        startGameButton.GetComponent<Button>().onClick.AddListener(
            () => {
                startGameButton.SetActive(false);
                quitButton.SetActive(false);
                titleText.enabled = false;
                SwitchToCancelTiles();
            });
        quitButton.GetComponent<Button>().onClick.AddListener(
            () => {
                Application.Quit();
            });
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
        playerHealthText.enabled = true;
        enemyHealthText.enabled = true;
        mainCam.transform.position = battleResolveCamPos;
        battleController.InitiateBattleResolution(comboController.GetCancelSeq());
    }

    private void SwitchToCancelTiles() {
        countingDown = true;
        playerHealthText.enabled = false;
        enemyHealthText.enabled = false;
        mainCam.transform.position = cancelTileCamPos;
    }

    private void SwitchToEdScreen(string setText) {
        startGameButton.SetActive(true);
        quitButton.SetActive(true);
        countingDown = false;
        titleText.text = setText;
        titleText.enabled = true;
        playerHealthText.enabled = false;
        enemyHealthText.enabled = false;
        battleController.ResetBattle();
        startGameButton.GetComponent<Button>().GetComponentInChildren<Text>().text = "Restart";
        mainCam.transform.position = opEdCamPos;
    }

    public void ChangeActiveState(string battleResult) {
        timer = 30.0f;
        boardController.ResetBoard();
        comboController.ClearCancelSequence();
        if (battleResult.Equals(BattleController.won)) {
            SwitchToEdScreen("You Win!");
        } else if (battleResult.Equals(BattleController.lost)) {
            SwitchToEdScreen("You Lost!");
        } else if (battleResult.Equals(BattleController.unresolved)) {
            SwitchToCancelTiles();
        }
    }
}
