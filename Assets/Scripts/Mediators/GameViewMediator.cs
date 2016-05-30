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

	public override void OnRegister()
	{
		battleWonSignal.AddListener(OnBattleWon);
		battleLostSignal.AddListener(OnBattleLost);
		battleUnresolvedSignal.AddListener(OnBattleUnresolved);
		// Place to add listeners for signals
		gameView.Init();
	}

	public void OnBattleWon()
	{
		gameView.ResetActiveState();
		gameView.SwitchToEdScreen("You Win!");
		resetBattleSignal.Dispatch();
	}

	public void OnBattleLost()
	{
		gameView.ResetActiveState();
		gameView.SwitchToEdScreen("You Lost!");
		resetBattleSignal.Dispatch();
	}

	public void OnBattleUnresolved()
	{
		gameView.ResetActiveState();
		gameView.SwitchToCancelTiles();
	}
}
