﻿using UnityEngine;
using System.Collections.Generic;

public class InBattlePlayerStatus : InBattleStatus {

    [Inject]
    public PlayerHealthUpdatedSignal receivedDmgSignal { get; set; }
    [Inject]
    public PlayerEquipComboUpdatedSignal equipComboUpdatedSignal { get; set; }
    [Inject]
    public EngageCombatSignal engageCombatSignal { get; set; }
    [Inject]
    public IPlayerStatus playerStatus { get; set; }

    protected override void FireHealthUpdatedSignal() {
        receivedDmgSignal.Dispatch();
    }

    protected override void FireEquipComboUpdatedSignal() {
        equipComboUpdatedSignal.Dispatch();
    }

    protected override void BindSignals() {
        engageCombatSignal.AddListener(UpdateInBattleStatus);
    }

    private void UpdateInBattleStatus(int enemyId) {
        maxHealth = playerStatus.Health;
        currentHealth = maxHealth;
        damage = playerStatus.Damage;

        UpdateEquipWeapon(playerStatus.GetEquippedWeapons());
    }
}
