using System;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;

public interface ISkillInitiator {

    void InvokeSkillFuncFromSkillId(int skillId, ActionParams parameters = null);
    void InjectInitialize(ICrossContextInjectionBinder injectionBinder);
}
