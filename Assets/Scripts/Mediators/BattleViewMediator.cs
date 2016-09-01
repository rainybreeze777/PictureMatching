using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

public class BattleViewMediator : Mediator {

    [Inject]
    public BattleView battleView{ get; set;}
    [Inject]
    public InitiateBattleResolutionSignal initiateBattleResolutionSignal { get; set; }
    [Inject]
    public ResetBattleSignal resetBattleSignal { get; set; }
    [Inject]
    public OneExchangeDoneSignal oneExchangeDoneSignal { get; set; }
    [Inject]
    public ResolveOneExchangeSignal resolveOneExchangeSignal { get; set; }
    [Inject]
    public IComboModel comboModel { get; set; }
    [Inject]
    public IEnemyModel enemyModel { get; set; }

    public override void OnRegister() {

        initiateBattleResolutionSignal.AddListener(InitiateBattleResolution);
        resetBattleSignal.AddListener(ResetBattle);
        oneExchangeDoneSignal.AddListener(OnOneExchangeDone);
        battleView.moveIsDone.AddListener(OnBattleViewFinishMoving);

        battleView.Init();
    }

    public void ResetBattle() {
        battleView.ResetBattle();
    }

    public void InitiateBattleResolution() {
        battleView.InitiateBattleResolution(comboModel.GetCancelSeq(), enemyModel.GetPrevGeneratedSequence());
        resolveOneExchangeSignal.Dispatch(true);
    }

    private void OnOneExchangeDone(Enum winner, int winnerTileNum) {
        battleView.UpdateResultTile(winnerTileNum);
    }

    private void OnBattleViewFinishMoving() {
        resolveOneExchangeSignal.Dispatch(false);
    }
}