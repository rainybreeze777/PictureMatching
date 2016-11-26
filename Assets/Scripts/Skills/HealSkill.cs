using System;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class HealSkill : ComboSkill {

    [Inject(EInBattleStatusType.PLAYER)]
    public IInBattleStatus playerBattleStatus { get; set; }

    protected override void ExecuteSkill() {
        return;
    }

    protected override void ExecuteBattleSkill() {
        // First verify that returned ActionParams object
        // contains the parameters this skill needs
        if (skillParams == null)
            throw new ArgumentNullException("HealSkill received null argument list from program");
        else if (skillParams.Count() < 1)
            throw new ArgumentException("HealSkill received argument list with length 0; Expecting arguments length 1");
        else if (skillParams.GetParamType(0) != typeof(int))
            throw new ArgumentException("HealSkill received argument of invalid type " + skillParams.GetParamType(0) + "; Expecting type int");
        // If ActionParams has more than 1 argument returned, warn and ignore
        else if (skillParams.Count() != 1) {
            Debug.LogWarning("HealSkill received more than 1 argument; Make sure this is the desired data");
        }

        int result = (int) skillParams.GetArg(0);
        playerBattleStatus.AddToHealth(result);
    }
}
