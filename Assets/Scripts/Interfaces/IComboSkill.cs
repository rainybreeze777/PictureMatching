using System;
using System.Collections.Generic;

public interface IComboSkill {

    void CancelStageExecute();
    void CancelStageExecuteWithArgs(ActionParams args);
    void BattleStageExecute();
    void BattleStageExecuteWithArgs(ActionParams args);
    bool NeedsUserInputData();
    void AbortExecution();
    bool AIDeduceIsLogicalToUse(SkillReqAndArg skillReqAndArg);
    void AIUseSkill(ActionParams args);
}
