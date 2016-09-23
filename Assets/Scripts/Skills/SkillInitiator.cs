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
    [Inject]
    public StartGameSignal gameStartSignal { get; set; }
    [Inject]
    public InitiateBattleResolutionSignal initiateBattleResolutionSignal { get; set; }
    [Inject]
    public BattleUnresolvedSignal battleUnresolvedSignal{ get; set;}

    private Dictionary<int, IComboSkill> skillMap = new Dictionary<int, IComboSkill>();

    private int keyCount = 0;

    private enum GameStage {
        CANCEL_STAGE, RESOLUTION_STAGE
    }
    private GameStage currentStage = GameStage.CANCEL_STAGE;

    public void InvokeSkillFuncFromSkillId(int skillId, ActionParams parameters = null) {
        if (currentStage == GameStage.CANCEL_STAGE) {
            skillMap[skillId].CancelStageExecuteWithArgs(boardModel, parameters);
        }
    }

    public SkillInitiator() {
    }

    [PostConstruct]
    public void PostConstruct() {
        gameStartSignal.AddListener(SwitchToCancelStage);
        battleUnresolvedSignal.AddListener(SwitchToCancelStage);
        initiateBattleResolutionSignal.AddListener(SwitchToResolutionStage);
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

    public void SwitchToCancelStage() { currentStage = GameStage.CANCEL_STAGE; }
    public void SwitchToResolutionStage() { currentStage = GameStage.RESOLUTION_STAGE; }

}
