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

    private Dictionary<ESceneChange, Button> eSceneChangeToMapButtonDict = new Dictionary<ESceneChange, Button>();
    private Dictionary<Button, ESceneChange> mapButtonToESceneChangeDict = new Dictionary<Button, ESceneChange>();

    public Signal swapToStatusButtonClickedSignal = new Signal();
    public Signal<ESceneChange> mapButtonClickedSignal = new Signal<ESceneChange>();

    internal void Init() {

        swapToStatusButton.onClick.AddListener(() => {
            swapToStatusButtonClickedSignal.Dispatch();
        });

        InitButtonDict(stage1Button, ESceneChange.STAGE1);
        InitButtonDict(stage2Button, ESceneChange.STAGE2);
    }

    public void ToggleSceneAvailability(ESceneChange sceneChange, bool isAvailable) {
        eSceneChangeToMapButtonDict[sceneChange].gameObject.SetActive(isAvailable);
    }

    public void ReconfigureAllScenesAvailability(List<ESceneChange> availableGameScenes) {
        foreach(KeyValuePair<ESceneChange, Button> kvp in eSceneChangeToMapButtonDict) {
            kvp.Value.gameObject.SetActive(false);
        }

        foreach(ESceneChange scene in availableGameScenes) {
            eSceneChangeToMapButtonDict[scene].gameObject.SetActive(true);
        }
    }

    private void InitButtonDict(Button button, ESceneChange sceneChange) {
        eSceneChangeToMapButtonDict.Add(sceneChange, button);
        mapButtonToESceneChangeDict.Add(button, sceneChange);
        button.onClick.AddListener(() => {
            mapButtonClickedSignal.Dispatch(mapButtonToESceneChangeDict[button]);
        });
        button.gameObject.SetActive(false);
    }
}
