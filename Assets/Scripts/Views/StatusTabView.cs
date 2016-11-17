using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

public class StatusTabView : View {

    [Inject]
    public StatusTabChangedSignal statusTabChangedSignal { get; set; }
    [Inject]
    public GameFlowStateChangeSignal gameFlowStateChangeSignal { get; set; }

    [SerializeField] private Toggle smeltToggle;
    [SerializeField] private Toggle equipToggle;

    private ToggleGroup toggleGroup;

    [PostConstruct]
    public void PostConstruct() {
        smeltToggle.onValueChanged.AddListener(OnSmeltToggleChanged);
        equipToggle.onValueChanged.AddListener(OnEquipToggleChanged);
        gameFlowStateChangeSignal.AddListener(OnGameFlowStateChange);


        toggleGroup = GetComponent<ToggleGroup>();
        smeltToggle.isOn = true;
    }

    // GameFlowStateChangeSignal will be fired when switching between map and status
    // usually this is when status panel first gets called, so everything will be
    // reset. Default SmeltView to be shown
    private void OnGameFlowStateChange(EGameFlowState gameFlowState) {
        if (gameFlowState != EGameFlowState.STATUS) { return; }

        smeltToggle.isOn = true;
        equipToggle.isOn = false;
    }

    private void OnSmeltToggleChanged(bool isOn) {
        statusTabChangedSignal.Dispatch(EStatusTab.SMELT, isOn);
    }

    private void OnEquipToggleChanged(bool isOn) {
        statusTabChangedSignal.Dispatch(EStatusTab.EQUIP, isOn);
    }
}
