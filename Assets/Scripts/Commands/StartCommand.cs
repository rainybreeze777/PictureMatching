using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.impl;

public class StartCommand : Command
{
	
	[Inject(ContextKeys.CONTEXT_VIEW)]
	public GameObject contextView{get;set;}
	
	public override void Execute()
	{
		GameObject GameViewObject = new GameObject();
		GameViewObject.name = "GameView";
		GameViewObject.AddComponent<GameView>();
		GameViewObject.transform.parent = contextView.transform;
	
		GameObject BattleViewObject = new GameObject();
		BattleViewObject.name = "BattleView";
		BattleViewObject.AddComponent<BattleView>();
		BattleViewObject.transform.parent = contextView.transform;
	}
}
