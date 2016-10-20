using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.command.impl;

// Triggered by StartGame signal
// different from StartSignal, where startSignal starts the executable
public class StartGameCommand : Command
{
    
    [Inject]
    public IEnemyModel enemyModel { get; set; }
    [Inject]
    public ResetBattleSignal resetBattleSignal { get; set; }
    [Inject]
    public ISkillInitiator skillInitiator { get; set; }

    public override void Execute()
    {
    	enemyModel.GenerateSequence();
    	skillInitiator.SwitchToCancelStage();
    	resetBattleSignal.Dispatch();
    }
}
