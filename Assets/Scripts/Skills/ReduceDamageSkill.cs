using System;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class ReduceDamageSkill : ComboSkill {

    protected override void ExecuteSkill() {
        return;
    }

    protected override void ExecuteBattleSkill() {
        // First verify that returned ActionParams object
        // contains the parameters this skill needs
        if (skillParams == null)
            throw new ArgumentNullException("ReduceDamageSkill received null argument list from program");
        else if (skillParams.Count() < 1)
            throw new ArgumentException("ReduceDamageSkill received argument list with length 0; Expecting arguments length 1");
        else if (skillParams.GetParamType(0) != typeof(double))
            throw new ArgumentException("ReduceDamageSkill received argument of invalid type " + skillParams.GetParamType(0) + "; Expecting type double");
        // If ActionParams has more than 1 argument returned, warn and ignore
        else if (skillParams.Count() != 1) {
            Debug.LogWarning("ReduceDamageSkill received more than 1 argument; Make sure this is the desired data");
        }

        Assert.IsTrue(battleStatus != null);
        battleStatus.UpdateReceiveDamageModifier((double) skillParams.GetArg(0));
    }
}
