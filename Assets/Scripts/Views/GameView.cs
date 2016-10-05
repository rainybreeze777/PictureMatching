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
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button endRoundButton;
    [SerializeField] private Text metalGathered;
    [SerializeField] private Text woodGathered;
    [SerializeField] private Text waterGathered;
    [SerializeField] private Text fireGathered;
    [SerializeField] private Text earthGathered;

    [SerializeField] private Button resButton1;
    [SerializeField] private Button resButton2;
    [SerializeField] private Button backButton;

    [SerializeField] private ProgressBar timeLeftPB;
    [SerializeField] private SpriteRenderer startMenuBackground;
    [SerializeField] private SpriteRenderer cancellationBackground;

    private const string START_SCREEN_KEY = "START_SCREEN";
    [SerializeField] private GameObject startScreenPanel;
    private const string GAME_KEY = "GAME";
    [SerializeField] private GameObject gamePanel;
    private const string CANCEL_STAGE_KEY = "CANCEL_STAGE";
    [SerializeField] private GameObject cancellationStageUIPanel;
    private const string BATTLE_RESOLVE_KEY = "BATTLE_RESOLVE";
    [SerializeField] private GameObject battleResolutionUIPanel;
    private const string START_MENU_KEY = "START_MENU";
    [SerializeField] private GameObject startMenuUIPanel;
    private const string RESOLUTION_KEY = "RESOLUTION";
    [SerializeField] private GameObject resolutionUIPanel;

    private RadioUIGroup flowControlGroup = new RadioUIGroup();
    private RadioUIGroup gameGroup = new RadioUIGroup();
    private RadioUIGroup startMenuGroup = new RadioUIGroup();

    public Signal endThisRoundSignal = new Signal();

    [Inject]
    public StartGameSignal gameStartSignal { get; set; }


    internal void Init() {
        Screen.SetResolution (1366, 768, false);

        flowControlGroup.AddToGroup(START_SCREEN_KEY, startScreenPanel);
        flowControlGroup.AddToGroup(GAME_KEY, gamePanel);
        gameGroup.AddToGroup(CANCEL_STAGE_KEY, cancellationStageUIPanel);
        gameGroup.AddToGroup(BATTLE_RESOLVE_KEY, battleResolutionUIPanel);
        startMenuGroup.AddToGroup(START_MENU_KEY, startMenuUIPanel);
        startMenuGroup.AddToGroup(RESOLUTION_KEY, resolutionUIPanel);

        mainCam = Camera.main;
        mainCam.transform.position = opEdCamPos;

        titleText.text = "";
        timeLeftPB.Value = 100;

        flowControlGroup.ActivateUI(START_SCREEN_KEY);
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
    }

    public void SwitchToBattleResolve() {
        gameGroup.ActivateUI(BATTLE_RESOLVE_KEY);
        mainCam.transform.position = battleResolveCamPos;
    }

    public void SwitchToCancelTiles() {
        flowControlGroup.ActivateUI(GAME_KEY);
        gameGroup.ActivateUI(CANCEL_STAGE_KEY);
        mainCam.transform.position = cancelTileCamPos;
    }

    private void SwitchToOptionsMenu() {
        startMenuGroup.ActivateUI(RESOLUTION_KEY);
    }

    private void SwitchToMainMenu() {
        titleText.text = "";
        flowControlGroup.ActivateUI(START_SCREEN_KEY);
        startMenuGroup.ActivateUI(START_MENU_KEY);
        mainCam.transform.position = opEdCamPos;
    }

    public void SwitchToEdScreen(string setText) {
        titleText.text = setText;
        flowControlGroup.ActivateUI(START_SCREEN_KEY);
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

    public void SetElementGathered(EElements elem, int count) {
        switch(elem) {
            case EElements.METAL:
                metalGathered.text = count.ToString();
                break;
            case EElements.WOOD:
                woodGathered.text = count.ToString();
                break;
            case EElements.WATER:
                waterGathered.text = count.ToString();
                break;
            case EElements.FIRE:
                fireGathered.text = count.ToString();
                break;
            case EElements.EARTH:
                earthGathered.text = count.ToString();
                break;
        }
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
