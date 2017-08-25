using UnityEngine;
using System.Collections.Generic;

public class BattleResolver : IBattleResolver {

    [Inject]
    public IComboModel comboModel { get; set; }
    [Inject]
    public IEnemyModel enemyModel { get; set; }
    [Inject]
    public ISkillInitiator skillInitiator { get; set; }
    [Inject(EInBattleStatusType.PLAYER)]
    public IInBattleStatus playerStatus { get; set; }
    [Inject(EInBattleStatusType.ENEMY)]
    public IInBattleStatus enemyStatus { get; set; }
    [Inject]
    public IProgressData progressData { get; set; }

    // Injected Signals
    [Inject]
    public BattleResultUpdatedSignal battleResultUpdatedSignal { get; set; }
    [Inject]
    public OneExchangeDoneSignal oneExchangeDoneSignal { get; set; }
    [Inject]
    public PlayerEssenceGainedSignal playerEssenceGainedSignal { get; set; }

    private int resolvingIndex = 0;
    private int afterVictoryEnableSceneId;
    private string wasVictoriousKey = "$was_victorious";

    public void ResolveNextMove() {

        EnemyPonderUseSkill();

        if (playerStatus.IsDead) {
            playerEssenceGainedSignal.Dispatch(new List<int>() {0, 0, 0, 0, 0});
            battleResultUpdatedSignal.Dispatch(EBattleResult.LOST);
            progressData.SetValue(wasVictoriousKey, false);
        } else if (enemyStatus.IsDead) {
            IInBattleEnemyStatus inBattleEnemyStatus = enemyStatus as IInBattleEnemyStatus;
            playerEssenceGainedSignal.Dispatch(inBattleEnemyStatus.GetRewardEssence());
            battleResultUpdatedSignal.Dispatch(EBattleResult.WON);
            progressData.SetValue(wasVictoriousKey, true);
        }

        if (playerStatus.IsDead || enemyStatus.IsDead) {
            resolvingIndex = 0;
            return;
        }

        List<int> playerSeq = comboModel.GetCancelSeq();
        List<int> enemySeq = enemyModel.GetPrevGeneratedSequence();

        if (resolvingIndex >= playerSeq.Count && resolvingIndex >= enemySeq.Count) {
            battleResultUpdatedSignal.Dispatch(EBattleResult.UNRESOLVED);
        }

        int playerMove = (resolvingIndex < playerSeq.Count) ? playerSeq[resolvingIndex] : -1;
        int enemyMove = (resolvingIndex < enemySeq.Count) ? enemySeq[resolvingIndex] : -1;

        if (playerMove != -1 || enemyMove != -1) {
            int compareResult = ElementResolver.ResolveAttack(playerMove, enemyMove);
            switch (compareResult) {
                case 0:
                    oneExchangeDoneSignal.Dispatch(EOneExchangeWinner.TIE, playerMove);
                    break;
                case 1:
                    enemyStatus.ReceiveDmg(playerStatus.Damage);
                    oneExchangeDoneSignal.Dispatch(EOneExchangeWinner.PLAYER, playerMove);
                    break;
                case 2:
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

    public List<EOneExchangeWinner> ForecastExchangeResult(int forecastSize) {
        List<EOneExchangeWinner> forecastResult = new List<EOneExchangeWinner>();

        List<int> playerSeq = comboModel.GetCancelSeq();
        List<int> enemySeq = enemyModel.GetPrevGeneratedSequence();

        if (resolvingIndex < playerSeq.Count || resolvingIndex < enemySeq.Count) {

            for(int i = resolvingIndex; i < resolvingIndex + forecastSize; ++i) {
                int playerMove = (i < playerSeq.Count) ? playerSeq[i] : -1;
                int enemyMove = (i < enemySeq.Count) ? enemySeq[i] : -1;

                if (playerMove != -1 || enemyMove != -1) {
                    int compareResult = ElementResolver.ResolveAttack(playerMove, enemyMove);
                    switch (compareResult) {
                        case 0:
                            forecastResult.Add(EOneExchangeWinner.TIE);
                            break;
                        case 1:
                            forecastResult.Add(EOneExchangeWinner.PLAYER);
                            break;
                        case 2:
                            forecastResult.Add(EOneExchangeWinner.ENEMY);
                            break;
                        case -1:
                            Debug.LogError("ForecastExchangeResult got Invalid Parameters!");
                            break;
                        default:
                            Debug.LogError("Unrecognized result!");
                            break;
                    }
                }
            }
        }

        return forecastResult;
    }

    private void EnemyPonderUseSkill() {
        List<int> reasonableSkills = skillInitiator.DeduceReasonableSkillsToUse(enemyModel.GetEnemyData());
        // For now execute all reasonable skills
        foreach(int skillId in reasonableSkills) {
            enemyModel.DeductSkillReqElems(skillId);
            skillInitiator.AIInvokeSkillFuncFromSkillId(
                skillId
                , enemyModel.GetEnemyData().GetSkillReqAndArgFromSkillId(skillId).Arguments);
        }
    }
}
