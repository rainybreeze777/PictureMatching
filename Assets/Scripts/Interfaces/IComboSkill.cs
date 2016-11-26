using System;
using System.Collections.Generic;

public interface IComboSkill {

    void CancelStageExecute();
    void CancelStageExecuteWithArgs(ActionParams args);
    void BattleStageExecute();
    void BattleStageExecuteWithArgs(ActionParams args);
    bool NeedsUserInputData();
    void AbortExecution();
}
