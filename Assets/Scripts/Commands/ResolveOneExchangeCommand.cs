using System;
using UnityEngine;
using strange.extensions.command.impl;

public class ResolveOneExchangeCommand : Command
{
    [Inject]
    public IBattleResolver battleResolver { get; set; }
    [Inject]
    public bool isNewRound { get; set; }

    public override void Execute() {
    	if (isNewRound) {
    		battleResolver.Reset();
    	}

    	battleResolver.ResolveNextMove();
    }
}
