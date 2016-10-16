using System;
using System.Collections.Generic;

public interface IComboSkill {

    void CancelStageExecute();
    void CancelStageExecute(IBoardModel boardModel);
    void CancelStageExecuteWithArgs(IBoardModel boardModel, ActionParams args);
    void BattleStageExecute();
    void BattleStageExecute(IInBattleStatus inBattleStatus);
    void BattleStageExecuteWithArgs(IInBattleStatus inBattleStatus, ActionParams args);
    bool NeedsUserInputData();
    void AbortExecution();
}
