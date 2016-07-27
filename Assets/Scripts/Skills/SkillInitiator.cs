using System;
using UnityEngine;
using System.Collections.Generic;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;

// A class used to map skillIds to the actual function implementations
// Other classes should obtain the skillId, and invoke the actual
// skill functions by passing Id into this class's methods
public class SkillInitiator : ISkillInitiator {

    [Inject]
    public IBoardModel boardModel { get; set; }

    private Dictionary<int, IComboSkill> skillMap = new Dictionary<int, IComboSkill>();

    public void InvokeSkillFuncFromSkillId(int skillId) {
        skillMap[skillId].CancelStageExecute(boardModel);
    }

    public SkillInitiator() {
        
    }

    // [PostConstruct]
    // public void PostConstruct() {
    //     Debug.LogWarning("SkillInitiator PostConstruct");
    //     // TODO: For now hard-code skillIds with each individual functions
    //     // Need more investigation as to whether there are better ways
    //     // to achieve this
    //     // IComboSkill comboSkill = new CancelColumnSkill(1);
    //     // skillMap.Add(1, comboSkill);
    // }

    public void InjectInitialize(ICrossContextInjectionBinder injectionBinder) {
        CancelColumnSkill comboSkill = new CancelColumnSkill(1);
        injectionBinder.injector.Inject(comboSkill);
        skillMap.Add(1, comboSkill);
    }

}
