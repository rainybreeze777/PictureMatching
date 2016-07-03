using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

public class GameView : View {

    private Camera mainCam;

    private Vector3 cancelTileCamPos = new Vector3(10.5f, 5f, -10f);
    private Vector3 battleResolveCamPos = new Vector3(10.5f, 20f, -10f);
    private Vector3 opEdCamPos = new Vector3(10.5f, -10f, -10f);

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

    private GameObject timeLeftPBPanel;
    private ProgressBar timeLeftPB;

    public Signal gameStartSignal = new Signal();

    internal void Init() {
        Screen.SetResolution (1024, 768, false);
        // battleController = GameObject.Find("BattleController").GetComponent<BattleController>() as BattleController;
        // comboController = GameObject.Find("ComboController").GetComponent<ComboController>() as ComboController;
        // boardController = GameObject.Find("BoardController").GetComponent<BoardController>() as BoardController;
    
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

        timeLeftPBPanel = GameObject.Find("TimeLeftPBPanel");
        timeLeftPB = GameObject.Find("TimeLeftPBFG").GetComponent<ProgressBar>();

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

        timeLeftPBPanel.SetActive(false);
        timeLeftPB.Value = 100;

        startGameButton.GetComponent<Button>().onClick.AddListener(
            () => {
                startGameButton.SetActive(false);
                optionButton.SetActive(false);
                quitButton.SetActive(false);
                titleText.enabled = false;
                gameStartSignal.Dispatch();
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

    public void SwitchToBattleResolve() {
        playerHealthText.enabled = true;
        enemyHealthText.enabled = true;
        mainCam.transform.position = battleResolveCamPos;
        timeLeftPBPanel.SetActive(false);
    }

    public void SwitchToCancelTiles() {
        playerHealthText.enabled = false;
        enemyHealthText.enabled = false;
        mainCam.transform.position = cancelTileCamPos;
        timeLeftPBPanel.SetActive(true);
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
        // countingDown = false;
        titleText.text = "Picture Matching";
        titleText.enabled = true;
        playerHealthText.enabled = false;
        enemyHealthText.enabled = false;
        resolutionText.enabled = false;
        // mediator.ResetBattle();
        mainCam.transform.position = opEdCamPos;
    }

    public void SwitchToEdScreen(string setText) {
        resButton1.SetActive(false);
        resButton2.SetActive(false);
        backButton.SetActive(false);
        startGameButton.SetActive(true);
        optionButton.SetActive(true);
        quitButton.SetActive(true);
        // countingDown = false;
        titleText.text = setText;
        titleText.enabled = true;
        playerHealthText.enabled = false;
        enemyHealthText.enabled = false;
        resolutionText.enabled = false;
        // mediator.ResetBattle();
        startGameButton.GetComponent<Button>().GetComponentInChildren<Text>().text = "Restart";
        mainCam.transform.position = opEdCamPos;
    }

    public void UpdateProgressBar(int percent) {
        timeLeftPB.Value = percent;
    }
}
