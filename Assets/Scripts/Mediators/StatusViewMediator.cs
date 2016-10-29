using System;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

public class StatusViewMediator : Mediator {

    [Inject]
    public StatusView statusView { get; set; }

    [Inject]
    public GameFlowStateChangeSignal gameFlowStateChangeSignal { get; set; }

    public override void OnRegister() {

    	statusView.swapToMapButtonClickedSignal.AddListener(OnSwapToMapClicked);

        statusView.Init();
    }

    private void OnSwapToMapClicked() {
    	gameFlowStateChangeSignal.Dispatch(EGameFlowState.MAP);
    }

}
