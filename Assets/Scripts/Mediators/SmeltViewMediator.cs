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
    [Inject]
    public PlayerWeaponsInfoUpdatedSignal weaponsInfoUpdatedSignal { get; set; }

    private bool waitingForSmeltResult = false;

    public override void OnRegister() {
        statusTabChangedSignal.AddListener(OnStatusTabChanged);
        smeltView.smeltButtonClickedSignal.AddListener(OnSmeltButtonClicked);
        weaponsInfoUpdatedSignal.AddListener(OnWeaponsInfoUpdated);

        smeltView.Init();
    }

    private void OnStatusTabChanged(EStatusTab statusTab, bool isOn) {
        if (statusTab != SmeltView.THIS_STATUS_TAB) { return; }

        smeltView.gameObject.SetActive(isOn);
    }
    
    private void OnSmeltButtonClicked(List<int> spentEssence) {
        waitingForSmeltResult = true;
        commenceSmeltSignal.Dispatch(spentEssence);
    }

    private void OnWeaponsInfoUpdated(EWeaponPossessionStatus status, Weapon w) {

        if (waitingForSmeltResult) {
            if (status == EWeaponPossessionStatus.ADD) {
                smeltView.SmeltObtainedWeapon(w);
            } else if ( status == EWeaponPossessionStatus.SMELT_INSUFFICIENT_ESSENCE) {
                smeltView.SmeltInsufficientEssence();
            }
        }
    }
}
