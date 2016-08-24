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

    private int keyCount = 0;

    public void InvokeSkillFuncFromSkillId(int skillId, ActionParams parameters = null) {
        skillMap[skillId].CancelStageExecuteWithArgs(boardModel, parameters);
    }

    public SkillInitiator() {
        
    }

    public void InjectInitialize(ICrossContextInjectionBinder injectionBinder) {
        // TODO: For now hard-code skillIds with each individual functions

        // *****ORDER MATTERS HERE, THE ORDER OF INJECTION DETERMINES *****
        // *****THE SKILL IDS *****

        // Need more investigation as to whether there are better ways
        // to achieve this
        
        // Skill 0
        InjectHelper(injectionBinder, new CancelSquare2By2Skill());
        // Skill 1
        InjectHelper(injectionBinder, new CancelColumnSkill());
        // Skill 2
        InjectHelper(injectionBinder, new AddToTimeSkill());
    }

    private void InjectHelper(ICrossContextInjectionBinder injectionBinder, IComboSkill comboSkill) {
        injectionBinder.injector.Inject(comboSkill);
        skillMap.Add(keyCount++, comboSkill);
    }

}
