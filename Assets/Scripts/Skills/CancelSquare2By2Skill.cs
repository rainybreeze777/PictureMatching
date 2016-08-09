using System;
using EUserInputDataRequests;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class CancelSquare2By2Skill : ComboSkill {

    [Construct]
    public CancelSquare2By2Skill()
    {
        NeedUserInput(BoardViewRequest.SELECT_SQUARE2);
    }

    protected override void ExecuteSkill() {

        // First verify that returned ActionParams object
        // contains the parameters this skill needs
        if (inputData == null)
            throw new ArgumentNullException("CancelSquare2By2Skill received null argument list");
        else if (inputData.Count() < 1)
            throw new ArgumentException("CancelSquare2By2Skill received argument list with length 0; Expecting arguments length 1");
        else if (inputData.GetParamType(0) != typeof(int))
            throw new ArgumentException("CancelSquare2By2Skill received argument of invalid type " + inputData.GetParamType(0) + "; Expecting type int");
        // If ActionParams has more than 1 argument returned, warn and ignore
        else if (inputData.Count() != 1) {
            Debug.LogWarning("CancelSquare2By2Skill received more than 1 argument; Make sure this is the desired data");
        }

        // If its null, something happened that turned
        // the boardModel reference to null, which should
        // never happen
        Assert.IsNotNull(boardModel);
        boardModel.removeColumn((int) inputData.GetArg(0));
    }

}
