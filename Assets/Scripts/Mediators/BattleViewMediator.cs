using System;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

public class BattleViewMediator : Mediator {
	[Inject]
	public BattleView battleView{ get; set;}

	public override void OnRegister() {
		battleView.Init();
	}

	public void ResetBattle() {
		battleView.ResetBattle();
	}
}
