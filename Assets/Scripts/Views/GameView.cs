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
    private Vector3 mapCamPos = new Vector3(10.5f, -25f, -10f);

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
    private const string STATUS_KEY = "STATUS";
    [SerializeField] private GameObject statusCanvas;
    private const string MAP_KEY = "MAP";
    [SerializeField] private GameObject mapCanvas;
    private const string BATTLE_END_KEY = "BATTLE_END";
    [SerializeField] private GameObject battleEndPanel;   

    private RadioUIGroup flowControlGroup = new RadioUIGroup();
    private RadioUIGroup gameGroup = new RadioUIGroup();
    private RadioUIGroup startMenuGroup = new RadioUIGroup();

    public Signal endThisRoundSignal = new Signal();

    internal void Init() {
        Screen.SetResolution (1366, 768, false);

        flowControlGroup.AddToGroup(START_SCREEN_KEY, startScreenPanel);
        flowControlGroup.AddToGroup(GAME_KEY, gamePanel);
        flowControlGroup.AddToGroup(STATUS_KEY, statusCanvas);
        flowControlGroup.AddToGroup(MAP_KEY, mapCanvas);
        flowControlGroup.AddToGroup(BATTLE_END_KEY, battleEndPanel);
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
                SwitchToMapScreen();
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

        battleEndPanel.GetComponent<BattleEndView>().clickedSignal.AddListener(SwitchToMapScreen);
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

    public void SwitchToOptionsMenu() {
        startMenuGroup.ActivateUI(RESOLUTION_KEY);
    }

    public void SwitchToMainMenu() {
        titleText.text = "";
        flowControlGroup.ActivateUI(START_SCREEN_KEY);
        startMenuGroup.ActivateUI(START_MENU_KEY);
        mainCam.transform.position = opEdCamPos;
    }

    public void SwitchToStatusScreen() {
        flowControlGroup.ActivateUI(STATUS_KEY);
        mainCam.transform.position = mapCamPos;
    }

    public void SwitchToBattleEndScreen() {
        flowControlGroup.ActivateUI(BATTLE_END_KEY);
        mainCam.transform.position = mapCamPos;
    }

    public void SwitchToMapScreen() {
        flowControlGroup.ActivateUI(MAP_KEY);
        mainCam.transform.position = mapCamPos;
    }

    public void UpdateProgressBar(float percent) {
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
