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
            skillMap[executing.skillIds.Peek()].CancelStageExecuteWithArgs(executing.skillParameters[0]);
        } else if (currentStage == GameStage.RESOLUTION_STAGE) {
            skillMap[executing.skillIds.Peek()].BattleStageExecuteWithArgs(executing.skillParameters[0]);
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

    private void InjectHelper(ICrossContextInjectionBinder injectionBinder, int skillId, IComboSkill comboSkill) {
        injectionBinder.injector.Inject(comboSkill);
        skillMap.Add(skillId, comboSkill);
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
                skillMap[executing.skillIds.Peek()].CancelStageExecuteWithArgs(executing.skillParameters[0]);
            } else if (currentStage == GameStage.RESOLUTION_STAGE) {
                skillMap[executing.skillIds.Peek()].BattleStageExecute();
            }
        } else {
            comboExecFinishedSignal.Dispatch(executing.comboId);
            executing = null;
        }
    }

    private void OnBattleResultUpdated(EBattleResult battleResult) {
        if (battleResult == EBattleResult.UNRESOLVED) {
            SwitchToCancelStage();
        }
    }
}
