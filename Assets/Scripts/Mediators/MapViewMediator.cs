using System;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

public class MapViewMediator : Mediator {

    [Inject]
    public MapView mapView { get; set; }

    [Inject]
    public IProgressData progressData { get; set; }

    [Inject]
    public SceneChangeSignal sceneChangeSignal { get; set; }
    [Inject]
    public GameFlowStateChangeSignal gameFlowStateChangeSignal { get; set; }
    [Inject]
    public AvailableScenesUpdateSignal availableScenesUpdateSignal { get; set; }

    public override void OnRegister() {

        mapView.mapButtonClickedSignal.AddListener(OnMapButtonClicked);
        mapView.swapToStatusButtonClickedSignal.AddListener(OnSwapToStatusClicked);

        availableScenesUpdateSignal.AddListener(OnAvailableScenesUpdated);

        mapView.Init();
    }

    private void OnMapButtonClicked(ESceneChange sceneChange) {
        sceneChangeSignal.Dispatch(sceneChange);
        gameFlowStateChangeSignal.Dispatch(EGameFlowState.SCENE);
    }

    private void OnSwapToStatusClicked() {
        gameFlowStateChangeSignal.Dispatch(EGameFlowState.STATUS);
    }

    private void OnAvailableScenesUpdated(ESceneChange scene, EAvailScenesUpdateType updateType) {
        switch (updateType) {
            case EAvailScenesUpdateType.ADD:
                mapView.ToggleSceneAvailability(scene, true);
                break;
            case EAvailScenesUpdateType.REMOVE:
                mapView.ToggleSceneAvailability(scene, false);
                break;
            case EAvailScenesUpdateType.BATCH_UPDATE:
                mapView.ReconfigureAllScenesAvailability(progressData.GetAvailableScenes());
                break;
            default:
                break;
        }
    }
}
