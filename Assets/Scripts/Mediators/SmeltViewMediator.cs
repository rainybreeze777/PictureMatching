using System;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

public class SmeltViewMediator : Mediator {

    [Inject]
    public SmeltView smeltView { get; set; }
    [Inject]
    public StatusTabChangedSignal statusTabChangedSignal { get; set; }
    [Inject]
    public GameFlowStateChangeSignal gameFlowStateChangeSignal { get; set; }
    [Inject]
    public CommenceSmeltSignal commenceSmeltSignal { get; set; }

    public override void OnRegister() {
        statusTabChangedSignal.AddListener(OnStatusTabChanged);
        gameFlowStateChangeSignal.AddListener(OnGameFlowStateChange);
        smeltView.smeltButtonClickedSignal.AddListener(OnSmetlButtonClicked);

        smeltView.Init();
    }

    private void OnStatusTabChanged(EStatusTab statusTab, bool isOn) {
        if (statusTab != SmeltView.THIS_STATUS_TAB) { return; }

        smeltView.gameObject.SetActive(isOn);
    }

    // GameFlowStateChangeSignal will be fired when switching between map and status
    // usually this is when status panel first gets called, so everything will be
    // reset. Default SmeltView to be shown
    private void OnGameFlowStateChange(EGameFlowState gameFlowState) {
        if (gameFlowState != EGameFlowState.STATUS) { return; }

        OnStatusTabChanged(SmeltView.THIS_STATUS_TAB, true);
    }

    private void OnSmetlButtonClicked(List<int> spentEssence) {
        commenceSmeltSignal.Dispatch(spentEssence);
    }
}
