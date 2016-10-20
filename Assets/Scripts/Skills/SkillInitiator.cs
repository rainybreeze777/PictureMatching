using System;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;

// A class used to map skillIds to the actual function implementations
// Other classes should obtain the skillId, and invoke the actual
// skill functions by passing Id into this class's methods
public class SkillInitiator : ISkillInitiator {

    [Inject]
    public IBoardModel boardModel { get; set; }
    [Inject(EInBattleStatusType.PLAYER)]
    public IInBattleStatus playerStatus { get; set; }
    [Inject(EInBattleStatusType.ENEMY)]
    public IInBattleStatus enemyStatus { get; set; }
    [Inject]
    public InitiateBattleResolutionSignal initiateBattleResolutionSignal { get; set; }
    [Inject]
    public BattleUnresolvedSignal battleUnresolvedSignal{ get; set;}
    [Inject]
    public ComboExecFinishedSignal comboExecFinishedSignal { get; set; }
    [Inject]
    public SkillExecFinishedSignal skillExecFinishedSignal { get; set; }

    private Dictionary<int, IComboSkill> skillMap = new Dictionary<int, IComboSkill>();
    private SkillGroup executing = null;

    private int keyCount = 0;

    private enum GameStage {
        CANCEL_STAGE, RESOLUTION_STAGE
    }
    private GameStage currentStage = GameStage.CANCEL_STAGE;

    public void InvokeSkillFuncFromSkillId(int comboId, int[] skillIds, List<ActionParams> parameters = null) {

        SkillGroup sg = new SkillGroup(comboId, skillIds, parameters);

        bool oneNeedsUserData = false;
        for (int i = 0; i < skillIds.Length; ++i) {
            bool thisOneNeedsUserData = skillMap[skillIds[i]].NeedsUserInputData();
            if (thisOneNeedsUserData && oneNeedsUserData) {
                throw new ArgumentException("SkillInitiator Error: Found a combo that has more than 1 skills requiring user input data simultaneously!");
            }
            if (thisOneNeedsUserData) {
                oneNeedsUserData = true;
                sg.needsData = true;
            }
        }
        
        if (executing != null && executing.needsData) {
            foreach (int skillId in executing.skillIds) {
                skillMap[skillId].AbortExecution();
            }
            executing = null;
        } else if (executing != null && !executing.needsData){
            return; // Ignore the new skill request
        }

        executing = sg;

        if (currentStage == GameStage.CANCEL_STAGE) {
            skillMap[executing.skillIds.Peek()].CancelStageExecuteWithArgs(boardModel, executing.skillParameters[0]);
        } else if (currentStage == GameStage.RESOLUTION_STAGE) {
            skillMap[executing.skillIds.Peek()].BattleStageExecuteWithArgs(playerStatus, executing.skillParameters[0]);
        }
    }

    public SkillInitiator() {
    }

    [PostConstruct]
    public void PostConstruct() {
        battleUnresolvedSignal.AddListener(SwitchToCancelStage);
        initiateBattleResolutionSignal.AddListener(SwitchToResolutionStage);
        skillExecFinishedSignal.AddListener(ExecuteNextSkill);
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
        // Skill 3
        InjectHelper(injectionBinder, new HealSkill());
        // Skill 4
        InjectHelper(injectionBinder, new ReduceDamageSkill());
    }

    public void SwitchToCancelStage() { currentStage = GameStage.CANCEL_STAGE; }
    public void SwitchToResolutionStage() { currentStage = GameStage.RESOLUTION_STAGE; }

    private void InjectHelper(ICrossContextInjectionBinder injectionBinder, IComboSkill comboSkill) {
        injectionBinder.injector.Inject(comboSkill);
        skillMap.Add(keyCount++, comboSkill);
    }

    private class SkillGroup {
        public int comboId;
        public Queue<int> skillIds;
        public List<ActionParams> skillParameters;
        public bool needsData;

        public SkillGroup(int comboId, int[] skillIds, List<ActionParams> parameters) {
            this.comboId = comboId;
            this.skillIds = new Queue<int>(skillIds);
            skillParameters = parameters;
            needsData = false;
        }
    }

    private void ExecuteNextSkill() {
        executing.skillIds.Dequeue();
        if (executing.skillIds.Count > 0) {
            if (currentStage == GameStage.CANCEL_STAGE) {
                skillMap[executing.skillIds.Peek()].CancelStageExecuteWithArgs(boardModel, executing.skillParameters[0]);
            } else if (currentStage == GameStage.RESOLUTION_STAGE) {
                skillMap[executing.skillIds.Peek()].BattleStageExecute();
            }
        } else {
            comboExecFinishedSignal.Dispatch(executing.comboId);
            executing = null;
        }
    }
}
