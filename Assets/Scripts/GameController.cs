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
    private Text resolutionText;
    private GameObject startGameButton;
    private GameObject optionButton;
    private GameObject quitButton;

    private GameObject resButton1;
    private GameObject resButton2;
    private GameObject backButton;

    void Awake () {
        Screen.SetResolution (1024, 768, false);
        battleController = GameObject.Find("BattleController").GetComponent<BattleController>() as BattleController;
        comboController = GameObject.Find("ComboController").GetComponent<ComboController>() as ComboController;
        boardController = GameObject.Find("BoardController").GetComponent<BoardController>() as BoardController;
    
        playerHealthText = GameObject.Find("PlayerHealth").GetComponent<Text>();
        enemyHealthText = GameObject.Find("EnemyHealth").GetComponent<Text>();
        titleText = GameObject.Find("TitleText").GetComponent<Text>();
        resolutionText = GameObject.Find("Resolution").GetComponent<Text>();

        startGameButton = GameObject.Find("StartGameButton");
        optionButton = GameObject.Find("OptionButton");
        quitButton = GameObject.Find("QuitButton");
        resButton1 = GameObject.Find("ResolutionButton1");
        resButton2 = GameObject.Find("ResolutionButton2");
        backButton = GameObject.Find("BackButton");
    }

    void Start () {
        mainCam = Camera.main;
        mainCam.transform.position = opEdCamPos;

        titleText.text = "Picture Matching";
        startGameButton.GetComponent<Button>().GetComponentInChildren<Text>().text = "Start Game";
        playerHealthText.enabled = false;
        enemyHealthText.enabled = false;
        resolutionText.enabled = false;
        
        resButton1.SetActive(false);
        resButton2.SetActive(false);
        backButton.SetActive(false);

        startGameButton.GetComponent<Button>().onClick.AddListener(
            () => {
                startGameButton.SetActive(false);
                optionButton.SetActive(false);
                quitButton.SetActive(false);
                titleText.enabled = false;
                SwitchToCancelTiles();
            });
        optionButton.GetComponent<Button>().onClick.AddListener(
            () => {
                startGameButton.SetActive(false);
                optionButton.SetActive(false);
                quitButton.SetActive(false);
                titleText.enabled = false;
                SwitchToOptionsMenu();
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

    private void SwitchToOptionsMenu() {
        resolutionText.enabled = true;
        resButton1.SetActive(true);
        resButton2.SetActive(true);
        backButton.SetActive(true);

        resButton1.GetComponent<Button>().onClick.AddListener(
            () => {
                Screen.SetResolution (1024, 768, false);
            });

        resButton2.GetComponent<Button>().onClick.AddListener(
            () => {
                Screen.SetResolution (1366, 768, false);
            });

        backButton.GetComponent<Button>().onClick.AddListener(
            () => {
                SwitchToMainMenu();
            });
    }

    private void SwitchToMainMenu() {
        resButton1.SetActive(false);
        resButton2.SetActive(false);
        backButton.SetActive(false);
        startGameButton.SetActive(true);
        optionButton.SetActive(true);
        quitButton.SetActive(true);
        countingDown = false;
        titleText.text = "Picture Matching";
        titleText.enabled = true;
        playerHealthText.enabled = false;
        enemyHealthText.enabled = false;
        resolutionText.enabled = false;
        battleController.ResetBattle();
        mainCam.transform.position = opEdCamPos;
    }

    private void SwitchToEdScreen(string setText) {
        resButton1.SetActive(false);
        resButton2.SetActive(false);
        backButton.SetActive(false);
        startGameButton.SetActive(true);
        optionButton.SetActive(true);
        quitButton.SetActive(true);
        countingDown = false;
        titleText.text = setText;
        titleText.enabled = true;
        playerHealthText.enabled = false;
        enemyHealthText.enabled = false;
        resolutionText.enabled = false;
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
