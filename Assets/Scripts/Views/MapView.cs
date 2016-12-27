using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;

public class MapView : View {

    [SerializeField] private Button stage1Button;
    [SerializeField] private Button stage2Button;
    [SerializeField] private Button swapToStatusButton;

    private Dictionary<int, Button> sceneIdToMapButtonDict = new Dictionary<int, Button>();
    private Dictionary<Button, int> mapButtonToSceneIdDict = new Dictionary<Button, int>();
    private Dictionary<Button, ESceneChange> mapButtonToESceneChangeDict = new Dictionary<Button, ESceneChange>();

    public Signal swapToStatusButtonClickedSignal = new Signal();
    public Signal<ESceneChange> mapButtonClickedSignal = new Signal<ESceneChange>();

    private int INITIAL_AVAIL_SCENE_ID = 1;

    internal void Init() {

        swapToStatusButton.onClick.AddListener(() => {
            swapToStatusButtonClickedSignal.Dispatch();
        });

        InitButtonDict(stage1Button, 1, ESceneChange.STAGE1);
        InitButtonDict(stage2Button, 2, ESceneChange.STAGE2);

        sceneIdToMapButtonDict[INITIAL_AVAIL_SCENE_ID].gameObject.SetActive(true);
    }

    public void ToggleSceneAvailability(int gameSceneId, bool isAvailable) {
        sceneIdToMapButtonDict[gameSceneId].gameObject.SetActive(isAvailable);
    }

    public void ReconfigureAllScenesAvailability(List<int> availableGameSceneIds) {
        foreach(KeyValuePair<int, Button> kvp in sceneIdToMapButtonDict) {
            kvp.Value.gameObject.SetActive(false);
        }

        foreach(int gameSceneId in availableGameSceneIds) {
            sceneIdToMapButtonDict[gameSceneId].gameObject.SetActive(true);
        }
    }

    private void InitButtonDict(Button button, int gameSceneId, ESceneChange sceneChange) {
        sceneIdToMapButtonDict.Add(gameSceneId, button);
        mapButtonToSceneIdDict.Add(button, gameSceneId);
        mapButtonToESceneChangeDict.Add(button, sceneChange);
        button.onClick.AddListener(() => {
            mapButtonClickedSignal.Dispatch(mapButtonToESceneChangeDict[button]);
        });
        button.gameObject.SetActive(false);
    }
}
