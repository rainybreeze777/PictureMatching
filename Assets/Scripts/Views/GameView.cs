using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

public class GameView : View {

    private Camera mainCam;

    private Vector3 cancelTileCamPos = new Vector3(10.5f, 5f, -10f);
    private Vector3 battleResolveCamPos = new Vector3(10.5f, 20f, -10f);
    private Vector3 opEdCamPos = new Vector3(10.5f, -10f, -10f);

    [SerializeField] private Text playerHealthText;
    [SerializeField] private Text enemyHealthText;
    [SerializeField] private Text titleText;
    [SerializeField] private Text resolutionText;
    [SerializeField] private GameObject startGameButton;
    [SerializeField] private GameObject optionButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject endRoundButton;
    [SerializeField] private GameObject comboButton;

    [SerializeField] private GameObject resButton1;
    [SerializeField] private GameObject resButton2;
    [SerializeField] private GameObject backButton;

    [SerializeField] private GameObject timeLeftPBPanel;
    [SerializeField] private ProgressBar timeLeftPB;

    private const string START_SCREEN_KEY = "START_SCREEN";
    [SerializeField] private GameObject startScreenPanel;
    private const string CANCEL_STAGE_KEY = "CANCEL_STAGE";
    [SerializeField] private GameObject cancellationStageUIPanel;
    private const string BATTLE_RESOLVE_KEY = "BATTLE_RESOLVE";
    [SerializeField] private GameObject battleResolutionUIPanel;
    private const string START_MENU_KEY = "START_MENU";
    [SerializeField] private GameObject startMenuUIPanel;
    private const string RESOLUTION_KEY = "RESOLUTION";
    [SerializeField] private GameObject resolutionUIPanel;

    private RadioUIGroup gameControlGroup = new RadioUIGroup();
    private RadioUIGroup startMenuGroup = new RadioUIGroup();

    public Signal gameStartSignal = new Signal();
    public Signal endThisRoundSignal = new Signal();

    [Inject]
    public MakeComboSignal makeComboSignal { get; set; }

    internal void Init() {
        Screen.SetResolution (1024, 768, false);

        // playerHealthText = GameObject.Find("PlayerHealth").GetComponent<Text>();
        // enemyHealthText = GameObject.Find("EnemyHealth").GetComponent<Text>();
        // titleText = GameObject.Find("TitleText").GetComponent<Text>();
        // resolutionText = GameObject.Find("Resolution").GetComponent<Text>();

        // startScreenPanel = GameObject.Find("StartScreenPanel");
        gameControlGroup.AddToGroup(START_SCREEN_KEY, startScreenPanel);
        // cancellationStageUIPanel = GameObject.Find("CancellationStageUIPanel");
        gameControlGroup.AddToGroup(CANCEL_STAGE_KEY, cancellationStageUIPanel);
        // battleResolutionUIPanel = GameObject.Find("BattleResolutionUIPanel");
        gameControlGroup.AddToGroup(BATTLE_RESOLVE_KEY, battleResolutionUIPanel);
        // startMenuUIPanel = GameObject.Find("StartMenuUIPanel");
        startMenuGroup.AddToGroup(START_MENU_KEY, startMenuUIPanel);
        // resolutionUIPanel = GameObject.Find("ResolutionUIPanel");
        startMenuGroup.AddToGroup(RESOLUTION_KEY, resolutionUIPanel);

        // startGameButton = GameObject.Find("StartGameButton");
        // optionButton = GameObject.Find("OptionButton");
        // quitButton = GameObject.Find("QuitButton");
        // resButton1 = GameObject.Find("ResolutionButton1");
        // resButton2 = GameObject.Find("ResolutionButton2");
        // backButton = GameObject.Find("BackButton");
        // endRoundButton = GameObject.Find("EndRoundButton");

        // timeLeftPBPanel = GameObject.Find("TimeLeftPBPanel");
        // timeLeftPB = GameObject.Find("TimeLeftPBFG").GetComponent<ProgressBar>();

        mainCam = Camera.main;
        mainCam.transform.position = opEdCamPos;

        titleText.text = "Picture Matching";
        startGameButton.GetComponent<Button>().GetComponentInChildren<Text>().text = "Start Game";
        // playerHealthText.enabled = false;
        // enemyHealthText.enabled = false;
        // resolutionText.enabled = false;
        
        // resButton1.SetActive(false);
        // resButton2.SetActive(false);
        // backButton.SetActive(false);
        // endRoundButton.SetActive(false);

        // timeLeftPBPanel.SetActive(false);
        timeLeftPB.Value = 100;

        gameControlGroup.ActivateUI(START_SCREEN_KEY);
        startMenuGroup.ActivateUI(START_MENU_KEY);

        startGameButton.GetComponent<Button>().onClick.AddListener(() => {
                // startGameButton.SetActive(false);
                // optionButton.SetActive(false);
                // quitButton.SetActive(false);
                // titleText.enabled = false;
                gameStartSignal.Dispatch();
            });
        optionButton.GetComponent<Button>().onClick.AddListener(() => {
                // startGameButton.SetActive(false);
                // optionButton.SetActive(false);
                // quitButton.SetActive(false);
                // titleText.enabled = false;
                SwitchToOptionsMenu();
            });
        quitButton.GetComponent<Button>().onClick.AddListener(() => {
                Application.Quit();
            });
        endRoundButton.GetComponent<Button>().onClick.AddListener(() => {
                endThisRoundSignal.Dispatch();
            });
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
        comboButton.SetActive(false);
        comboButton.GetComponent<Button>().onClick.AddListener( 
            () => {
                comboButton.SetActive(false);
                makeComboSignal.Dispatch();
            });
    }

    public void SwitchToBattleResolve() {
        gameControlGroup.ActivateUI(BATTLE_RESOLVE_KEY);
        // startScreenPanel.SetActive(false);
        // playerHealthText.enabled = true;
        // enemyHealthText.enabled = true;
        mainCam.transform.position = battleResolveCamPos;
        // timeLeftPBPanel.SetActive(false);
        // endRoundButton.SetActive(false);
    }

    public void SwitchToCancelTiles() {
        gameControlGroup.ActivateUI(CANCEL_STAGE_KEY);
        // playerHealthText.enabled = false;
        // enemyHealthText.enabled = false;
        mainCam.transform.position = cancelTileCamPos;
        // timeLeftPBPanel.SetActive(true);
        // endRoundButton.SetActive(true);
    }

    private void SwitchToOptionsMenu() {
        // resolutionText.enabled = true;
        // resButton1.SetActive(true);
        // resButton2.SetActive(true);
        // backButton.SetActive(true);
        startMenuGroup.ActivateUI(RESOLUTION_KEY);
    }

    private void SwitchToMainMenu() {
        // resButton1.SetActive(false);
        // resButton2.SetActive(false);
        // backButton.SetActive(false);
        // startGameButton.SetActive(true);
        // optionButton.SetActive(true);
        // quitButton.SetActive(true);
        titleText.text = "Picture Matching";
        gameControlGroup.ActivateUI(START_SCREEN_KEY);
        // titleText.enabled = true;
        // playerHealthText.enabled = false;
        // enemyHealthText.enabled = false;
        // resolutionText.enabled = false;
        mainCam.transform.position = opEdCamPos;
    }

    public void SwitchToEdScreen(string setText) {
        // resButton1.SetActive(false);
        // resButton2.SetActive(false);
        // backButton.SetActive(false);
        // startGameButton.SetActive(true);
        // optionButton.SetActive(true);
        // quitButton.SetActive(true);
        // countingDown = false;
        titleText.text = setText;
        // titleText.enabled = true;
        // playerHealthText.enabled = false;
        // enemyHealthText.enabled = false;
        // resolutionText.enabled = false;
        // mediator.ResetBattle();
        startGameButton.GetComponent<Button>().GetComponentInChildren<Text>().text = "Restart";
        gameControlGroup.ActivateUI(START_SCREEN_KEY);
        mainCam.transform.position = opEdCamPos;
    }

    public void UpdateProgressBar(int percent) {
        timeLeftPB.Value = percent;
    }

    public void UpdatePlayerHealthText(string text) {
        playerHealthText.text = text;
    }

    public void UpdateEnemyHealthText(string text) {
        enemyHealthText.text = text;
    }

    public void ComboButtonSetActive(bool active) {
        comboButton.SetActive(active);
    }

    private class RadioUIGroup {
        private Dictionary<string, GameObject> uiGroup = new Dictionary<string, GameObject>();

        public void AddToGroup(string name, GameObject ui) {
            uiGroup.Add(name, ui);
        }

        public void DeactivateAll() {
            foreach(KeyValuePair<string, GameObject> kvp in uiGroup) {
                kvp.Value.SetActive(false);
            }
        }

        public void ActivateUI(string name) {
            DeactivateAll();
            uiGroup[name].SetActive(true);
        }
    }
}
