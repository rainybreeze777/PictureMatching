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
    
    public override void Execute()
    {
    	enemyModel.GenerateSequence();
    }
}
