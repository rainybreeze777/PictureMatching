using UnityEngine;
using System.Collections.Generic;

public class InBattlePlayerStatus : InBattleStatus {

    [Inject]
    public PlayerHealthUpdatedSignal receivedDmgSignal { get; set; }
    [Inject]
    public PlayerEquipWeaponUpdatedSignal equipWeaponUpdatedSignal { get; set; }
    [Inject]
    public PlayerEquipComboUpdatedSignal equipComboUpdatedSignal { get; set; }

    protected override void FireHealthUpdatedSignal() {
        receivedDmgSignal.Dispatch();
    }

    protected override void FireEquipComboUpdatedSignal() {
    	equipComboUpdatedSignal.Dispatch();
    }

    protected override void BindSignals() {
    	equipWeaponUpdatedSignal.AddListener(UpdateEquipWeapon);
    }
}
