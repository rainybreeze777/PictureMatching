﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

public class InGameMenuViewMediator : Mediator {

    [Inject]
    public InGameMenuView inGameMenuView { get; set; }
    [Inject]
    public IGameStateMachine gameStateMachine { get; set; }

    [Inject]
    public EscKeyPressedSignal escKeyPressedSignal { get; set; }

    public override void OnRegister() {

        escKeyPressedSignal.AddListener(OnEscKeyPressed);
        inGameMenuView.backButtonClickedSignal.AddListener(OnEscKeyPressed);

        gameObject.SetActive(false);

        inGameMenuView.Init();
    }

    private void OnEscKeyPressed() {

        if (gameStateMachine.CurrentState == EGameFlowState.SCENE
            || gameStateMachine.CurrentState == EGameFlowState.MAP
            || gameStateMachine.CurrentState == EGameFlowState.STATUS) {
            inGameMenuView.OnEscKeyPressed();
        }
    }
}