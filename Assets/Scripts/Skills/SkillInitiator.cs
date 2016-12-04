using System;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;
using SimpleJSON;

// A class used to map skillIds to the actual function implementations
// Other classes should obtain the skillId, and invoke the actual
// skill functions by passing Id into this class's methods
public class SkillInitiator : ISkillInitiator {

    [Inject]
    public InitiateBattleResolutionSignal initiateBattleResolutionSignal { get; set; }
    [Inject]
    public BattleResultUpdatedSignal battleResultUpdatedSignal { get; set; }
    [Inject]
    public ComboExecFinishedSignal comboExecFinishedSignal { get; set; }
    [Inject]
    public SkillExecFinishedSignal skillExecFinishedSignal { get; set; }

    private Dictionary<int, IComboSkill> skillMap = new Dictionary<int, IComboSkill>();
    private SkillGroup executing = null;

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
                sg.NeedsData = true;
            }
        }
        
        if (executing != null && executing.NeedsData) {
            Queue<int> remainingSkillIds = executing.GetRemainingSkillIds();
            foreach (int skillId in remainingSkillIds) {
                skillMap[skillId].AbortExecution();
            }
            executing = null;
        } else if (executing != null && !executing.NeedsData){
            return; // Ignore the new skill request
        }

        executing = sg;

        if (currentStage == GameStage.CANCEL_STAGE) {
            skillMap[executing.PeekSkillId()].CancelStageExecuteWithArgs(executing.PeekActionParams());
        } else if (currentStage == GameStage.RESOLUTION_STAGE) {
            skillMap[executing.PeekSkillId()].BattleStageExecuteWithArgs(executing.PeekActionParams());
        }
    }

    [PostConstruct]
    public void PostConstruct() {
        battleResultUpdatedSignal.AddListener(OnBattleResultUpdated);
        initiateBattleResolutionSignal.AddListener(SwitchToResolutionStage);
        skillExecFinishedSignal.AddListener(ExecuteNextSkill);
    }

    public void InjectInitialize(ICrossContextInjectionBinder injectionBinder) {

        JSONClass skillIdToSkillClass = JSON.Parse((Resources.Load("Skills") as TextAsset).text)["skillIdToSkillClass"] as JSONClass;

        foreach(KeyValuePair<string, JSONNode> node in skillIdToSkillClass ) {
            try {
                Type classType = Type.GetType(node.Value);
                IComboSkill skill = Activator.CreateInstance(classType) as IComboSkill;
                int skillId = Int32.Parse(node.Key);
                InjectHelper(injectionBinder, skillId, skill);
            } catch (Exception ex) {
                Debug.LogError(ex.ToString());
            }
        }
    }

    public void SwitchToCancelStage() { currentStage = GameStage.CANCEL_STAGE; }
    public void SwitchToResolutionStage() { currentStage = GameStage.RESOLUTION_STAGE; }

    public List<int> DeduceReasonableSkillsToUse(EnemyData enemyData) {

        List<int> reasonableSkills = new List<int>();
        List<int> availableSkills = enemyData.SkillIds;

        foreach(int skillId in availableSkills) {
            if (skillMap[skillId].AIDeduceIsLogicalToUse(enemyData.GetSkillReqAndArgFromSkillId(skillId))) {
                reasonableSkills.Add(skillId);
            }
        }

        return reasonableSkills;
    }

    public void AIInvokeSkillFuncFromSkillId(int skillId, ActionParams parameters = null) {
        skillMap[skillId].AIUseSkill(parameters);
    }

    private void InjectHelper(ICrossContextInjectionBinder injectionBinder, int skillId, IComboSkill comboSkill) {
        injectionBinder.injector.Inject(comboSkill);
        skillMap.Add(skillId, comboSkill);
    }

    private class SkillGroup {
        private int comboId;
        private Queue<int> skillIds;
        private Queue<ActionParams> skillParameters;
        private bool needsData;

        public int ComboId { get { return comboId; } }
        public bool NeedsData { 
            get { return needsData; } 
            set { needsData = value; }
        }

        public int PeekSkillId() { return skillIds.Peek(); }
        public ActionParams PeekActionParams() { return skillParameters.Peek(); }
        public void Dequeue() {
            skillIds.Dequeue();
            skillParameters.Dequeue();
        }

        public Queue<int> GetRemainingSkillIds() { return new Queue<int>(skillIds); }
        public int RemainingSkillIdsCount() { return skillIds.Count; }

        public SkillGroup(int comboId, int[] skillIds, List<ActionParams> parameters) {
            this.comboId = comboId;
            this.skillIds = new Queue<int>(skillIds);
            skillParameters = new Queue<ActionParams>(parameters);
            needsData = false;
        }
    }

    private void ExecuteNextSkill() {
        executing.Dequeue();
        if (executing.RemainingSkillIdsCount() > 0) {
            if (currentStage == GameStage.CANCEL_STAGE) {
                skillMap[executing.PeekSkillId()].CancelStageExecuteWithArgs(executing.PeekActionParams());
            } else if (currentStage == GameStage.RESOLUTION_STAGE) {
                skillMap[executing.PeekSkillId()].BattleStageExecuteWithArgs(executing.PeekActionParams());
            }
        } else {
            comboExecFinishedSignal.Dispatch(executing.ComboId);
            executing = null;
        }
    }

    private void OnBattleResultUpdated(EBattleResult battleResult) {
        if (battleResult == EBattleResult.UNRESOLVED) {
            SwitchToCancelStage();
        }
    }
}
