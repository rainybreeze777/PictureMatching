using System;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

public class MapViewMediator : Mediator {

    [Inject]
    public MapView mapView{ get; set;}

    [Inject]
    public IBiographer playerBiographer { get; set; }

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

    private void OnAvailableScenesUpdated(int gameSceneId, EAvailScenesUpdateType updateType) {
        switch (updateType) {
            case EAvailScenesUpdateType.ADD:
                mapView.ToggleSceneAvailability(gameSceneId, true);
                break;
            case EAvailScenesUpdateType.REMOVE:
                mapView.ToggleSceneAvailability(gameSceneId, false);
                break;
            case EAvailScenesUpdateType.BATCH_UPDATE:
                mapView.ReconfigureAllScenesAvailability(playerBiographer.GetAllAvailableSceneIds());
                break;
            default:
                break;
        }
    }
}
