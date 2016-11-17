using System;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

public class GameViewMediator : Mediator {

    [Inject]
    public GameView gameView{ get; set;}
    [Inject(EInBattleStatusType.PLAYER)]
    public IInBattleStatus playerStatus { get; set; }
    [Inject(EInBattleStatusType.ENEMY)]
    public IInBattleStatus enemyStatus { get; set; }

    // Injected Signals
    [Inject]
    public BattleResultUpdatedSignal battleResultUpdatedSignal { get; set; }
    [Inject]
    public BoardIsEmptySignal boardIsEmptySignal { get; set; }
    [Inject]
    public ResetActiveStateSignal resetActiveStateSignal { get; set; }
    [Inject]
    public InitiateBattleResolutionSignal initiateBattleResolutionSignal { get; set; }
    [Inject]
    public PlayerHealthUpdatedSignal playerHealthUpdatedSignal { get; set; }
    [Inject]
    public EnemyHealthUpdatedSignal enemyHealthUpdatedSignal { get; set; }
    [Inject]
    public AddToTimeSignal addToTimeSignal { get; set; }
    [Inject]
    public EngageCombatSignal engageCombatSignal { get; set; }
    [Inject]
    public ElemGatherUpdatedSignal elemGatherUpdatedSignal { get; set; }
    [Inject]
    public GameFlowStateChangeSignal gameFlowStateChangeSignal { get; set; }

    private const float TIME_PER_CANCEL = 60.0f;
    private float timer = TIME_PER_CANCEL;
    private bool countingDown = false;

    void Update () {
        if (countingDown) {
            timer -= Time.deltaTime;
            gameView.UpdateProgressBar(Mathf.Min(timer, TIME_PER_CANCEL) / TIME_PER_CANCEL * 100);
        }

        if (timer <= 0 && countingDown) {
            SwitchToBattleResolve();
        }
    }

    public override void OnRegister()
    {
        // Place to add listeners for signals
        battleResultUpdatedSignal.AddListener(OnBattleResultUpdated);
        boardIsEmptySignal.AddListener(SwitchToBattleResolve);
        engageCombatSignal.AddListener(SwitchToCancelTiles);
        gameView.endThisRoundSignal.AddListener(SwitchToBattleResolve);
        playerHealthUpdatedSignal.AddListener(OnPlayerHealthUpdate);
        enemyHealthUpdatedSignal.AddListener(OnEnemyHealthUpdate);
        addToTimeSignal.AddListener(AddToTimer);
        elemGatherUpdatedSignal.AddListener(OnElementGatherUpdated);
        gameFlowStateChangeSignal.AddListener(OnGameFlowStateChange);

        gameView.Init();
    }

    private void OnBattleResultUpdated(EBattleResult battleResult) {
        if (battleResult == EBattleResult.WON || battleResult == EBattleResult.LOST) {
            ResetActiveState();
            gameView.SwitchToBattleEndScreen();
        } else if (battleResult == EBattleResult.UNRESOLVED) {
#if !UNLIMITED_TIME
            countingDown = true;
#endif
            ResetActiveState();
            gameView.SwitchToCancelTiles();
        }
    }

    public void AddToTimer(double seconds) {
        timer += (float) seconds;
    }

    private void ResetActiveState()
    {
        timer = TIME_PER_CANCEL;
        resetActiveStateSignal.Dispatch();
    }

    private void SwitchToBattleResolve()
    {
        countingDown = false;
        gameView.SwitchToBattleResolve();
        initiateBattleResolutionSignal.Dispatch();
    }

    private void SwitchToCancelTiles()
    {
#if !UNLIMITED_TIME
        countingDown = true;
#endif
        gameView.UpdateProgressBar(100.0f);
        gameView.SwitchToCancelTiles();
    }

    private void OnPlayerHealthUpdate()
    {
        gameView.UpdatePlayerHealthText(playerStatus.CurrentHealth + " / " + playerStatus.MaxHealth);
    }

    private void OnEnemyHealthUpdate()
    {
        gameView.UpdateEnemyHealthText(enemyStatus.CurrentHealth + " / " + enemyStatus.MaxHealth);
    }

    private void OnElementGatherUpdated(EElements elem, int updateTo) {
        gameView.SetElementGathered(elem, updateTo);
    }

    private void OnGameFlowStateChange(EGameFlowState state) {
        switch(state) {
            case EGameFlowState.MAP:
                gameView.SwitchToMapScreen();
                break;
            case EGameFlowState.STATUS:
                gameView.SwitchToStatusScreen();
                break;
            default:
                break;
        }
    }
}
