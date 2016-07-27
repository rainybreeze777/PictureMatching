using System;
using EUserInputDataRequests;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class CancelColumnSkill : ComboSkill {

    public CancelColumnSkill(int skillId)
        : base(skillId)
    {
        NeedUserInput(BoardViewRequest.SELECT_COL);
    }

    [Construct]
    public CancelColumnSkill() : base(2)
    {
        // dataResponseSignal.AddListener(AwaitUserInput);
        // Debug.LogWarning("CancelColumnSkill Constructor done");
    }

    // [PostConstruct]
    // public void PostConstruct() {
    //     Debug.LogWarning("CancelColumnSkill Post Construct done");
    // }

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

        // If its null, something happened that turned
        // the boardModel reference to null, which should
        // never happen
        Assert.IsNotNull(boardModel);
        boardModel.removeColumn((int) inputData.GetArg(0));
    }

}
