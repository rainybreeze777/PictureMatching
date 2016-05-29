using System;
using UnityEngine;
using strange.extensions.mediation.impl;

public class GameViewMediator : Mediator {
	[Inject]
	public GameView gameView{ get; set;}

	public override void OnRegister()
	{
		// Place to add listeners for signals
		gameView.Init();
	}
}
