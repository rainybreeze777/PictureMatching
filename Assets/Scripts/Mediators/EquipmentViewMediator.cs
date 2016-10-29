﻿using System;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

public class EquipmentViewMediator : Mediator {

    [Inject]
    public EquipmentView equipmentView { get; set; }
    [Inject]
    public MapChangeSignal mapChangeSignal { get; set; }
    [Inject]
    public PlayerEquipWeaponUpdatedSignal equipWeaponUpdatedSignal { get; set; }

    public override void OnRegister() {

        equipmentView.confirmEquipSignal.AddListener(OnConfirmEquip);

        equipmentView.Init();
    }

    private void OnConfirmEquip() {
        equipWeaponUpdatedSignal.Dispatch(equipmentView.GetEquippedWeapons());
        // mapChangeSignal.Dispatch(EMapChange.MAP);
    }

}
