using System;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

public class MapViewMediator : Mediator {

    [Inject]
    public MapView mapView{ get; set;}

    [Inject]
    public MapChangeSignal mapChangeSignal { get; set; }
    [Inject]
    public GameFlowStateChangeSignal gameFlowStateChangeSignal { get; set; }

    public override void OnRegister() {

        mapView.mapButtonClickedSignal.AddListener(OnMapButtonClicked);
        mapView.swapToStatusButtonClickedSignal.AddListener(OnSwapToStatusClicked);
        mapView.Init();
    }

    private void OnMapButtonClicked(EMapChange mapChange) {
        mapChangeSignal.Dispatch(mapChange);
    }

    private void OnSwapToStatusClicked() {
        gameFlowStateChangeSignal.Dispatch(EGameFlowState.STATUS);
    }
}
