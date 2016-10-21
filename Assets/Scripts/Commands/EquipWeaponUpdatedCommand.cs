using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using strange.extensions.context.api;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.impl;

public class EquipWeaponUpdatedCommand : Command
{
    [Inject]
    public IComboModel comboModel{ get; set; }
    [Inject(EInBattleStatusType.PLAYER)]
    public IInBattleStatus playerStatus { get; set; }

    [Inject]
    public List<Weapon> equippedWeaponsList { get; set; }

    public override void Execute()
    {
        playerStatus.UpdateEquipWeapon(equippedWeaponsList);
        comboModel.RefreshEquippedCombo();
    }
}
