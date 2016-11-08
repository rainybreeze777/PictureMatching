using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

public class StatusTabView : View {

    [Inject]
    public StatusTabChangedSignal statusTabChangedSignal { get; set; }

    [SerializeField] private Toggle smeltToggle;
    [SerializeField] private Toggle equipToggle;

    private ToggleGroup toggleGroup;

    [PostConstruct]
    public void PostConstruct() {
        smeltToggle.onValueChanged.AddListener(OnSmeltToggleChanged);
        equipToggle.onValueChanged.AddListener(OnEquipToggleChanged);

        toggleGroup = GetComponent<ToggleGroup>();
        toggleGroup.SetAllTogglesOff();
        smeltToggle.isOn = true;
    }

    private void OnSmeltToggleChanged(bool isOn) {
        statusTabChangedSignal.Dispatch(EStatusTab.SMELT, isOn);
    }

    private void OnEquipToggleChanged(bool isOn) {
        statusTabChangedSignal.Dispatch(EStatusTab.EQUIP, isOn);
    }
}
