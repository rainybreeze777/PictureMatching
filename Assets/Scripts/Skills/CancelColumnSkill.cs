using System;
using EUserInputDataRequests;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class CancelColumnSkill : ComboSkill {

    [Inject]
    public IBoardModel playerBoardModel { get; set; }

    [Construct]
    public CancelColumnSkill()
    {
        NeedUserInput(BoardViewRequest.SELECT_COL);
    }

    public override bool AIDeduceIsLogicalToUse() {
        // This skill is only valid for Player to use
        // because AI currently has no cancel stage
        return false;
    }

    public override void AIUseSkill() {
        return;
    }

    protected override void ExecuteSkill() {

        // First verify that returned ActionParams object
        // contains the parameters this skill needs
        if (inputData == null)
            throw new ArgumentNullException("CancelColumnSkill received null argument list");
        else if (inputData.Count() < 1)
            throw new ArgumentException("CancelColumnSkill received argument list with length 0; Expecting arguments length 1");
        else if (inputData.GetParamType(0) != typeof(int))
            throw new ArgumentException("CancelColumnSkill received argument of invalid type " + inputData.GetParamType(0) + "; Expecting type int");
        // If ActionParams has more than 1 argument returned, warn and ignore
        else if (inputData.Count() != 1) {
            Debug.LogWarning("CancelColumnSkill received more than 1 argument; Make sure this is the desired data");
        }

        playerBoardModel.removeColumn((int) inputData.GetArg(0));
    }

    protected override void ExecuteBattleSkill() {
        return; // Nothing here
    }

}
