using System;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;

public interface ISkillInitiator {

    void InvokeSkillFuncFromSkillId(int skillId);
    void InjectInitialize(ICrossContextInjectionBinder injectionBinder);
}
