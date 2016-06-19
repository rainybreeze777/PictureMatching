using System;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

public class GameViewMediator : Mediator {
    [Inject]
    public GameView gameView{ get; set;}

    // Injected Signals
    [Inject]
    public BattleWonSignal battleWonSignal{ get; set;}
    [Inject]
    public BattleLostSignal battleLostSignal{ get; set;}
    [Inject]
    public BattleUnresolvedSignal battleUnresolvedSignal{ get; set;}
    [Inject]
    public ResetBattleSignal resetBattleSignal{ get; set; }
    [Inject]
    public BoardIsEmptySignal boardIsEmptySignal { get; set; }
    [Inject]
    public ResetActiveStateSignal resetActiveStateSignal { get; set; }
    [Inject]
    public InitiateBattleResolutionSignal initiateBattleResolutionSignal { get; set; }

    private float timer = 30.0f;
    private bool countingDown = false;

    void Update () {
        if (countingDown)
            timer -= Time.deltaTime;

        if (timer <= 0 && countingDown) {
            SwitchToBattleResolve();
        }
    }

    public override void OnRegister()
    {
        // Place to add listeners for signals
        battleWonSignal.AddListener(OnBattleWon);
        battleLostSignal.AddListener(OnBattleLost);
        battleUnresolvedSignal.AddListener(OnBattleUnresolved);
        boardIsEmptySignal.AddListener(SwitchToBattleResolve);       

        gameView.Init();
    }

    public void OnBattleWon()
    {
        ResetActiveState();
        gameView.SwitchToEdScreen("You Win!");
        resetBattleSignal.Dispatch();
    }

    public void OnBattleLost()
    {
        ResetActiveState();
        gameView.SwitchToEdScreen("You Lost!");
        resetBattleSignal.Dispatch();
    }

    public void OnBattleUnresolved()
    {
        ResetActiveState();
        countingDown = true;
        gameView.SwitchToCancelTiles();
    }

    private void ResetActiveState()
    {
        timer = 30.0f;
        resetActiveStateSignal.Dispatch();
    }

    private void SwitchToBattleResolve()
    {
        countingDown = false;
        gameView.SwitchToBattleResolve();
        initiateBattleResolutionSignal.Dispatch();
        Debug.Log("GameViewMediator SwitchToBattleResolve");
    }
}
