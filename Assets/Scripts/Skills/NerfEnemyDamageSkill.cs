using System;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class NerfEnemyDamageSkill : ComboSkill {

    [Inject(EInBattleStatusType.PLAYER)]
    public IInBattleStatus playerBattleStatus { get; set; }
    [Inject(EInBattleStatusType.ENEMY)]
    public IInBattleStatus enemyBattleStatus { get; set; }
    [Inject]
    public IBattleResolver battleResolver { get; set; }

    protected override bool AIDeduceIsLogicalToUseLogic(ActionParams args) {

        ArgumentCheck(args);

        int windowSize = (int) args.GetArg(1);
        List<EOneExchangeWinner> forecastResult = battleResolver.ForecastExchangeResult(windowSize);
        if (forecastResult.Count < windowSize - 1) {
            // Don't use the skill if there are not enough exchange
            // left for this round, save it for later
            return false;
        }

        // Don't the skill if player is not going to win for more than half
        // of the next windowSize exchanges
        int PlayerWinCount = 0;
        for (int i = 0; i < forecastResult.Count; ++i) {
            PlayerWinCount += (forecastResult[i] == EOneExchangeWinner.PLAYER) ? 1 : 0;
        }
        if (PlayerWinCount <= (int) (forecastResult.Count / 2)) {
            return false;
        }

        return true;
    }

    // This function should only be called after calling AIDeduceIsLogicalToUseLogic
    public override void AIUseSkill(ActionParams args) {
        Debug.Log("NerfEnemyDamageSkill used!");
        playerBattleStatus.UpdateDealDamageModifier((double) args.GetArg(0), (int) args.GetArg(1));
        return;
    }

    protected override void ExecuteSkill() {
        return;
    }

    protected override void ExecuteBattleSkill() {

        ArgumentCheck(skillParams);

        enemyBattleStatus.UpdateDealDamageModifier((double) skillParams.GetArg(0), (int) skillParams.GetArg(1));
    }

    private void ArgumentCheck(ActionParams args) {
        // First verify that returned ActionParams object
        // contains the parameters this skill needs
        if (args == null)
            throw new ArgumentNullException("NerfEnemyDamageSkill received null argument list from program");
        else if (args.Count() < 1)
            throw new ArgumentException("NerfEnemyDamageSkill received argument list with length 0; Expecting arguments length 1");
        else if (args.GetParamType(0) != typeof(int))
            throw new ArgumentException("NerfEnemyDamageSkill received argument of invalid type " + skillParams.GetParamType(0) + "; Expecting type int");
        // If ActionParams has more than 1 argument returned, warn and ignore
        else if (args.Count() != 1) {
            Debug.LogWarning("NerfEnemyDamageSkill received more than 1 argument; Make sure this is the desired data");
        }
    }
}
