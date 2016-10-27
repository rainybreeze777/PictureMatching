using System;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

public class MapViewMediator : Mediator {

    [Inject]
    public MapView mapView{ get; set;}

    [Inject]
    public MapChangeSignal mapChangeSignal { get; set; }

    public override void OnRegister() {

        mapView.mapButtonClickedSignal.AddListener(OnMapButtonClicked);

        mapView.Init();
    }

    public void OnMapButtonClicked(EMapChange mapChange) {
        mapChangeSignal.Dispatch(mapChange);
    }
}
