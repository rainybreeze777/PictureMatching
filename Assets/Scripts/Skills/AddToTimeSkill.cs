using System;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class AddToTimeSkill : ComboSkill {

    [Inject]
    public AddToTimeSignal addToTimeSignal { get; set; }

    protected override bool AIDeduceIsLogicalToUseLogic(ActionParams args) {
        // This skill is only valid for Player to use
        // because AI currently has no cancel stage
        return false;
    }

    public override void AIUseSkill(ActionParams args) {
        return;
    }

    protected override void ExecuteSkill() {

        // First verify that returned ActionParams object
        // contains the parameters this skill needs
        if (skillParams == null)
            throw new ArgumentNullException("AddToTimeSkill received null argument list from program");
        else if (skillParams.Count() < 1)
            throw new ArgumentException("AddToTimeSkill received argument list with length 0; Expecting arguments length 1");
        else if (skillParams.GetParamType(0) != typeof(double))
            throw new ArgumentException("AddToTimeSkill received argument of invalid type " + skillParams.GetParamType(0) + "; Expecting type double");
        // If ActionParams has more than 1 argument returned, warn and ignore
        else if (skillParams.Count() != 1) {
            Debug.LogWarning("AddToTimeSkill received more than 1 argument; Make sure this is the desired data");
        }

        addToTimeSignal.Dispatch((double) skillParams.GetArg(0));
    }

    protected override void ExecuteBattleSkill() {
        return; // Nothing here
    }
}
