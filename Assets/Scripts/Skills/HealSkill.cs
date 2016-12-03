using System;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class HealSkill : ComboSkill {

    [Inject(EInBattleStatusType.PLAYER)]
    public IInBattleStatus playerBattleStatus { get; set; }
    [Inject(EInBattleStatusType.ENEMY)]
    public IInBattleStatus enemyBattleStatus { get; set; }

    protected override bool AIDeduceIsLogicalToUseLogic(ActionParams args) {

        ArgumentCheck(args);

        int result = (int) args.GetArg(0);
        return enemyBattleStatus.CurrentHealth + result <= enemyBattleStatus.MaxHealth;
    }

    // This function should only be called after calling AIDeduceIsLogicalToUseLogic
    public override void AIUseSkill(ActionParams args) {
        Debug.Log("HealSkill used!");
        int result = (int) args.GetArg(0);
        enemyBattleStatus.AddToHealth(result);
        return;
    }

    protected override void ExecuteSkill() {
        return;
    }

    protected override void ExecuteBattleSkill() {

        ArgumentCheck(skillParams);

        int result = (int) skillParams.GetArg(0);
        playerBattleStatus.AddToHealth(result);
    }

    private void ArgumentCheck(ActionParams args) {
        // First verify that returned ActionParams object
        // contains the parameters this skill needs
        if (args == null)
            throw new ArgumentNullException("HealSkill received null argument list from program");
        else if (args.Count() < 1)
            throw new ArgumentException("HealSkill received argument list with length 0; Expecting arguments length 1");
        else if (args.GetParamType(0) != typeof(int))
            throw new ArgumentException("HealSkill received argument of invalid type " + skillParams.GetParamType(0) + "; Expecting type int");
        // If ActionParams has more than 1 argument returned, warn and ignore
        else if (args.Count() != 1) {
            Debug.LogWarning("HealSkill received more than 1 argument; Make sure this is the desired data");
        }
    }
}
