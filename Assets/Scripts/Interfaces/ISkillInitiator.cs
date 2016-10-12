using System;
using System.Collections.Generic;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;

public interface ISkillInitiator {

    void InvokeSkillFuncFromSkillId(int comboId, int[] skillIds, List<ActionParams> parametersList = null);
    void InjectInitialize(ICrossContextInjectionBinder injectionBinder);
}
