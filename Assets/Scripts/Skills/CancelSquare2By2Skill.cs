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
        else if (inputData.Count() < 4)
            throw new ArgumentException("CancelSquare2By2Skill received argument list with length < 4; Expecting arguments length 4");
        else {
            bool invalidArgumentType = false;
            for (int i = 0; i < 4; ++i) {
                if (inputData.GetParamType(i) != typeof(int)) {
                    invalidArgumentType = true;
                    break;
                }
            }

            if (invalidArgumentType) {
                throw new ArgumentException(
                    "CancelSquare2By2Skill received argument of invalid type " + 
                    inputData.GetParamType(0) + 
                    ", " + inputData.GetParamType(1) + 
                    ", " + inputData.GetParamType(2) + 
                    ", " + inputData.GetParamType(3) + 
                    "; Expecting type int, int, int, int");
            } else if (inputData.Count() != 4) {
                // If ActionParams has more than 4 argument returned, warn and ignore
                Debug.LogWarning("CancelSquare2By2Skill received more than 4 argument; Make sure this is the desired data");
            }
        }

        // If its null, something happened that turned
        // the boardModel reference to null, which should
        // never happen
        Assert.IsNotNull(boardModel);
        boardModel.removeRange(
            (int) inputData.GetArg(0),
            (int) inputData.GetArg(1),
            (int) inputData.GetArg(2),
            (int) inputData.GetArg(3));
    }

    protected override void ExecuteBattleSkill() {
        return; // Nothing here
    }

}
