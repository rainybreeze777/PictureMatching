﻿using System;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class ReduceDamageSkill : ComboSkill {

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

        // Don't the skill when enemy is about to win more than half
        // of the next windowSize exchanges
        int playerWinCount = 0;
        for (int i = 0; i < forecastResult.Count; ++i) {
            playerWinCount += (forecastResult[i] == EOneExchangeWinner.PLAYER) ? 1 : 0;
        }
        if (playerWinCount <= (int) (forecastResult.Count / 2)) {
            return false;
        }

        return true;
    }

    public override void AIUseSkill(ActionParams args) {
        Debug.Log("ReduceDamageSkill used!");
        enemyBattleStatus.UpdateReceiveDamageModifier((double) args.GetArg(0), (int) args.GetArg(1));
        return;
    }

    protected override void ExecuteSkill() {
        return;
    }

    protected override void ExecuteBattleSkill() {
        ArgumentCheck(skillParams);

        playerBattleStatus.UpdateReceiveDamageModifier((double) skillParams.GetArg(0), (int) skillParams.GetArg(1));
    }

    private void ArgumentCheck(ActionParams args) {
        // First verify that returned ActionParams object
        // contains the parameters this skill needs
        if (args == null)
            throw new ArgumentNullException("ReduceDamageSkill received null argument list from program");
        else if (args.Count() < 2)
            throw new ArgumentException("ReduceDamageSkill received argument list with length " + skillParams.Count() + "; Expecting arguments length 2");
        else if (args.GetParamType(0) != typeof(double))
            throw new ArgumentException("ReduceDamageSkill received argument of invalid type " + skillParams.GetParamType(0) + " at argument 0; Expecting type double");
        else if (args.GetParamType(1) != typeof(int))
            throw new ArgumentException("ReduceDamageSkill received argument of invalid type " + skillParams.GetParamType(1) + " at argument 1; Expecting type int");
        // If ActionParams has more than 1 argument returned, warn and ignore
        else if (args.Count() != 2) {
            Debug.LogWarning("ReduceDamageSkill received more than 2 argument; Make sure this is the desired data");
        }
    }
}
