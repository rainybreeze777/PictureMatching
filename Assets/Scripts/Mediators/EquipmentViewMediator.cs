using System;
using System.Collections.Generic;
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
    [Inject]
    public PlayerWeaponsInfoUpdatedSignal weaponsInfoUpdatedSignal { get; set; }

    [Inject]
    public IPlayerStatus playerStatus { get; set; }

    public override void OnRegister() {

        equipmentView.Init();

        equipmentView.confirmEquipSignal.AddListener(OnConfirmEquip);
        weaponsInfoUpdatedSignal.AddListener(OnWeaponsInfoUpdated);

        equipmentView.RefreshEquipmentView(playerStatus.GetPossessedWeapons(), playerStatus.GetEquippedWeapons());
    }

    private void OnConfirmEquip() {
        equipWeaponUpdatedSignal.Dispatch(equipmentView.GetEquippedWeapons());
    }

    private void OnWeaponsInfoUpdated(EWeaponPossessionStatus status, Weapon w) {
        if (status != EWeaponPossessionStatus.SMELT_INSUFFICIENT_ESSENCE) {
            equipmentView.RefreshEquipmentView(playerStatus.GetPossessedWeapons(), playerStatus.GetEquippedWeapons());
        }
    }
}
