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
    // [SerializeField] private GameObject comboButton;

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

    public Signal endThisRoundSignal = new Signal();

    // [Inject]
    // public MakeComboSignal makeComboSignal { get; set; }
    [Inject]
    public StartGameSignal gameStartSignal { get; set; }


    internal void Init() {
        Screen.SetResolution (1366, 768, false);

        gameControlGroup.AddToGroup(START_SCREEN_KEY, startScreenPanel);
        gameControlGroup.AddToGroup(CANCEL_STAGE_KEY, cancellationStageUIPanel);
        gameControlGroup.AddToGroup(BATTLE_RESOLVE_KEY, battleResolutionUIPanel);
        startMenuGroup.AddToGroup(START_MENU_KEY, startMenuUIPanel);
        startMenuGroup.AddToGroup(RESOLUTION_KEY, resolutionUIPanel);

        mainCam = Camera.main;
        mainCam.transform.position = opEdCamPos;

        titleText.text = "Picture Matching";
        startGameButton.GetComponent<Button>().GetComponentInChildren<Text>().text = "Start Game";
        timeLeftPB.Value = 100;

        gameControlGroup.ActivateUI(START_SCREEN_KEY);
        startMenuGroup.ActivateUI(START_MENU_KEY);

        startGameButton.GetComponent<Button>().onClick.AddListener(() => {
                gameStartSignal.Dispatch();
            });
        optionButton.GetComponent<Button>().onClick.AddListener(() => {
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
        /*
        comboButton.SetActive(false);
        comboButton.GetComponent<Button>().onClick.AddListener( 
            () => {
                comboButton.SetActive(false);
                makeComboSignal.Dispatch();
            });
            */
    }

    public void SwitchToBattleResolve() {
        gameControlGroup.ActivateUI(BATTLE_RESOLVE_KEY);
        mainCam.transform.position = battleResolveCamPos;
    }

    public void SwitchToCancelTiles() {
        gameControlGroup.ActivateUI(CANCEL_STAGE_KEY);
        mainCam.transform.position = cancelTileCamPos;
    }

    private void SwitchToOptionsMenu() {
        startMenuGroup.ActivateUI(RESOLUTION_KEY);
    }

    private void SwitchToMainMenu() {
        titleText.text = "Picture Matching";
        gameControlGroup.ActivateUI(START_SCREEN_KEY);
        startMenuGroup.ActivateUI(START_MENU_KEY);
        mainCam.transform.position = opEdCamPos;
    }

    public void SwitchToEdScreen(string setText) {
        titleText.text = setText;
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

    // public void ComboButtonSetActive(bool active) {
    //     comboButton.SetActive(active);
    // }

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
