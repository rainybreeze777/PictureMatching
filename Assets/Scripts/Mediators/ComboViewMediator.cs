﻿using System;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

public class ComboViewMediator : Mediator {

    [Inject]
    public ComboView view{ get; set;}
    [Inject]
    public IComboModel comboModel { get; set; }
    [Inject]
    public EnemySeqGenSignal enemySeqGenSignal { get; set; }
    [Inject]
    public IEnemyModel enemyModel { get; set; }
    [Inject]
    public BattleResultUpdatedSignal battleResultUpdatedSignal { get; set; }

    public override void OnRegister() {
        view.Init();

        comboModel.CancelAddedSignal.AddListener(OnTileCancelled);
        battleResultUpdatedSignal.AddListener(ResetRound);
        enemySeqGenSignal.AddListener(OnEnemySeqGeneration);
    }

    public void OnTileCancelled(int tileNumber) {
        view.AddToCancelSequence(tileNumber);
    }

    public void OnEnemySeqGeneration() {
        view.ConstructNewEnemySequence(enemyModel.GetPrevGeneratedSequence(), enemyModel.GetPrevSequenceMask());
    }

    public void ResetRound(EBattleResult result) {
        view.Reset();
    }
}
