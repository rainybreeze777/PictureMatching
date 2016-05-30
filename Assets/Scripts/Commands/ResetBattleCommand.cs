using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.impl;

public class ResetBattleCommand : Command {

	[Inject]
	public BattleViewMediator battleViewMediator { get; set; }

	public override void Execute() {
		battleViewMediator.ResetBattle();
	}
}
