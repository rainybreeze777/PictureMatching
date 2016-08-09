using System;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using strange.extensions.dispatcher.eventdispatcher.api;

public class ComboSkill : IComboSkill {

    [Inject]
    public UserInputDataRequestSignal dataRequestSignal { get; set; } 
    [Inject]
    public UserInputDataResponseSignal dataResponseSignal { get; set; }

    protected Enum userInputEnum = null;
    protected ActionParams inputData = null;
    protected IBoardModel boardModel = null;

    private bool needInputData = false;
    private bool requestingData = false;

    public void CancelStageExecute() {
        CancelStageExecute(null);
    }
    public void CancelStageExecute(IBoardModel boardModel) {
        this.boardModel = boardModel;
        if (needInputData) {
            Assert.IsNotNull(userInputEnum);
            RequestUserInputData(userInputEnum);
            // ExecuteSkill() will be run after receiving data
            // in function AwaitUserInput()
        } else {
            ExecuteSkill();
            boardModel = null;
        }
    }

    protected virtual void ExecuteSkill() {
    }

    protected void NeedUserInput (Enum userInputType) {
        needInputData = true;
        userInputEnum = userInputType;
    }

    protected ComboSkill() {
    }

    [PostConstruct]
    public void SignalBinder() {
        dataResponseSignal.AddListener(AwaitUserInput);
        Debug.LogWarning("ComboSkill postConstructor done");
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
            boardModel = null;
            inputData = null;
        }
    }
}
