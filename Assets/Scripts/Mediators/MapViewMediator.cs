using System;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

public class MapViewMediator : Mediator {

    [Inject]
    public MapView mapView{ get; set;}

    [Inject]
    public SceneChangeSignal sceneChangeSignal { get; set; }
    [Inject]
    public GameFlowStateChangeSignal gameFlowStateChangeSignal { get; set; }

    public override void OnRegister() {

        mapView.mapButtonClickedSignal.AddListener(OnMapButtonClicked);
        mapView.swapToStatusButtonClickedSignal.AddListener(OnSwapToStatusClicked);
        mapView.Init();
    }

    private void OnMapButtonClicked(ESceneChange sceneChange) {
        sceneChangeSignal.Dispatch(sceneChange);
        gameFlowStateChangeSignal.Dispatch(EGameFlowState.SCENE);
    }

    private void OnSwapToStatusClicked() {
        gameFlowStateChangeSignal.Dispatch(EGameFlowState.STATUS);
    }
}
