﻿using System;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using strange.extensions.dispatcher.eventdispatcher.api;

abstract public class ComboSkill : IComboSkill {

    [Inject]
    public UserInputDataRequestSignal dataRequestSignal { get; set; } 
    [Inject]
    public UserInputDataResponseSignal dataResponseSignal { get; set; }
    [Inject]
    public SkillExecFinishedSignal skillExecFinishedSignal { get; set; }

    protected Enum userInputEnum = null;
    protected ActionParams inputData = null;
    protected ActionParams skillParams = null;

    private bool needUserInputData = false;
    private bool requestingData = false;

    public void CancelStageExecute() {
        CancelStageExecuteWithArgs(null);
    }

    public void CancelStageExecuteWithArgs(ActionParams args) {

        skillParams = args;

        if (needUserInputData) {
            Assert.IsNotNull(userInputEnum);
            RequestUserInputData(userInputEnum);
            // ExecuteSkill() will be run after receiving data
            // in function AwaitUserInput()
        } else {
            ExecuteSkill();
            skillExecFinishedSignal.Dispatch();
        }
    }

    public void BattleStageExecute() {
        BattleStageExecuteWithArgs(null);
    }

    public void BattleStageExecuteWithArgs(ActionParams args) {
        skillParams = args;
        ExecuteBattleSkill();
        skillExecFinishedSignal.Dispatch();
    }

    public bool NeedsUserInputData() {
        return needUserInputData;
    }

    public void AbortExecution() {
        if (requestingData) {
            requestingData = false;
            inputData = null;
        }
    }

    abstract public void AIUseSkill();
    abstract public bool AIDeduceIsLogicalToUse();

    [PostConstruct]
    public void SignalBinder() {
        dataResponseSignal.AddListener(AwaitUserInput);
    }

    abstract protected void ExecuteSkill();
    abstract protected void ExecuteBattleSkill();

    protected void NeedUserInput (Enum userInputType) {
        needUserInputData = true;
        userInputEnum = userInputType;
    }

    protected ComboSkill() {
    }

    private void RequestUserInputData (Enum userInputType) {
        requestingData = true;
        dataRequestSignal.Dispatch(userInputType);
    }

    private void AwaitUserInput(Enum userInputType, ActionParams paramsList) {
        // The signal is always being listened to;
        // Only execute functions when this class actually requested
        // them, and make sure the response data is for this class
        if (requestingData && userInputType.Equals(userInputEnum)) {
            requestingData = false;
            inputData = paramsList;
            ExecuteSkill();
            skillExecFinishedSignal.Dispatch();
            inputData = null;
        } else if (requestingData && userInputType.Equals(ESkillStatus.INPUT_CANCELLED)) {
            AbortExecution();
        }
    }
}
