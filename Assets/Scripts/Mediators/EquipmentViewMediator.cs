using System;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

public class EquipmentViewMediator : Mediator {

    [Inject]
    public EquipmentView equipmentView { get; set; }
    [Inject]
    public StartGameSignal startGameSignal { get; set; }
    [Inject]
    public EquipWeaponUpdatedSignal equipWeaponUpdatedSignal { get; set; }

    public override void OnRegister() {

        equipmentView.confirmEquipSignal.AddListener(OnConfirmEquip);

        equipmentView.Init();
    }

    private void OnConfirmEquip() {
        equipWeaponUpdatedSignal.Dispatch(equipmentView.GetEquippedWeapons());
        startGameSignal.Dispatch();
    }

}
