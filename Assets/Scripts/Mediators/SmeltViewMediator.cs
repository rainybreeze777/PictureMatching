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
    public CommenceSmeltSignal commenceSmeltSignal { get; set; }

    public override void OnRegister() {
        statusTabChangedSignal.AddListener(OnStatusTabChanged);
        smeltView.smeltButtonClickedSignal.AddListener(OnSmetlButtonClicked);

        smeltView.Init();
    }

    private void OnStatusTabChanged(EStatusTab statusTab, bool isOn) {
        if (statusTab != SmeltView.THIS_STATUS_TAB) { return; }

        smeltView.gameObject.SetActive(isOn);
    }
    
    private void OnSmetlButtonClicked(List<int> spentEssence) {
        commenceSmeltSignal.Dispatch(spentEssence);
    }
}
