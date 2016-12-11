using System;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.command.impl;

public class EngageCombatCommand : Command
{
    
    [Inject]
    public IEnemyModel enemyModel { get; set; }
    [Inject]
    public ResetBattleSignal resetBattleSignal { get; set; }
    [Inject]
    public ISkillInitiator skillInitiator { get; set; }
    [Inject]
    public IInBattleEnemyStatus enemyStatus { get; set; }
    [Inject]
    public GameFlowStateChangeSignal gameFlowStateChangeSignal { get; set; }
    [Inject]
    public int enemyId { get; set; }
    [Inject]
    public List<int> injectedEssence { get; set; }

    private EnemyDataFetcher enemyDataFetcher = EnemyDataFetcher.GetInstance();

    public override void Execute()
    {
        EnemyData theEnemy = enemyDataFetcher.GetEnemyDataById(enemyId);
        enemyModel.SetUpEnemyData(theEnemy);
    	enemyModel.GenerateSequence();
        enemyStatus.InitWithEnemyData(theEnemy);
    	skillInitiator.SwitchToCancelStage();
    	resetBattleSignal.Dispatch();
        gameFlowStateChangeSignal.Dispatch(EGameFlowState.CANCELLATION);
    }
}
