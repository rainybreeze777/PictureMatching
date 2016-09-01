using UnityEngine;
using System.Collections.Generic;

public class BattleResolver : IBattleResolver {

    [Inject]
    public IComboModel comboModel { get; set; }
    [Inject]
    public IEnemyModel enemyModel { get; set; }
    // Injected Signals
    [Inject]
    public BattleWonSignal battleWonSignal { get; set; }
    [Inject]
    public BattleLostSignal battleLostSignal { get; set; }
    [Inject]
    public BattleUnresolvedSignal battleUnresolvedSignal { get; set; }
    [Inject]
    public OneExchangeDoneSignal oneExchangeDoneSignal { get; set; }

    [Inject(EInBattleStatusType.PLAYER)]
    public IInBattleStatus playerStatus { get; set; }
    [Inject(EInBattleStatusType.ENEMY)]
    public IInBattleStatus enemyStatus { get; set; }

    private int resolvingIndex = 0;

    public void ResolveNextMove() {

        if (playerStatus.IsDead) {
            battleLostSignal.Dispatch();
        } else if (enemyStatus.IsDead) {
            battleWonSignal.Dispatch();
        }

        if (playerStatus.IsDead || enemyStatus.IsDead) {
            resolvingIndex = 0;
            Debug.LogWarning("Skipping because someone's dead");
            return;
        }

        List<int> playerSeq = comboModel.GetCancelSeq();
        List<int> enemySeq = enemyModel.GetPrevGeneratedSequence();

        if (resolvingIndex >= playerSeq.Count && resolvingIndex >= enemySeq.Count) {
            battleUnresolvedSignal.Dispatch();
        }

        int playerMove = (resolvingIndex < playerSeq.Count) ? playerSeq[resolvingIndex] : -1;
        int enemyMove = (resolvingIndex < enemySeq.Count) ? enemySeq[resolvingIndex] : -1;

        if (playerMove != -1 || enemyMove != -1) {
            int compareResult = ElementResolver.ResolveAttack(playerMove, enemyMove);
            switch (compareResult) {
                case 0:
                    Debug.Log("Ties exchange " + (resolvingIndex + 1));
                    oneExchangeDoneSignal.Dispatch(EOneExchangeWinner.TIE, playerMove);
                    break;
                case 1:
                    Debug.Log("Player wins exchange " + (resolvingIndex + 1 ));
                    enemyStatus.ReceiveDmg(playerStatus.Damage);
                    oneExchangeDoneSignal.Dispatch(EOneExchangeWinner.PLAYER, playerMove);
                    break;
                case 2:
                    Debug.Log("Enemy wins exchange " + (resolvingIndex + 1));
                    playerStatus.ReceiveDmg(enemyStatus.Damage);
                    oneExchangeDoneSignal.Dispatch(EOneExchangeWinner.ENEMY, enemyMove);
                    break;
                case -1:
                    Debug.LogError("ResolveNextMove got Invalid Parameters!");
                    break;
                default:
                    Debug.LogError("Unrecognized result!");
                    break;
            }
        }

        resolvingIndex++;
    }

    public void Reset() {
        resolvingIndex = 0;
    }
}
