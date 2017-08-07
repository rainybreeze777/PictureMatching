using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

public class SceneViewMediator : Mediator {

    [Inject]
    public SceneView sceneView{ get; set;}

    [Inject]
    public SceneChangeSignal sceneChangeSignal { get; set; }
    [Inject]
    public EngageCombatSignal engageCombatSignal { get; set; }
    [Inject]
    public GameFlowStateChangeSignal gameFlowStateChangeSignal { get; set; }

    private Dictionary<ESceneChange, string> scripts = new Dictionary<ESceneChange, string>();

    private int gameSceneId = -1;

    public override void OnRegister() {

        // Hand-wire the mapping because Unity3D
        // is too dumb to serialize a dictionary
        // Always make sure the mappings are correct
        scripts.Add(ESceneChange.STAGE1, "Scene1");
        scripts.Add(ESceneChange.STAGE2, "Scene2");

        sceneChangeSignal.AddListener(OnSceneChange);
        sceneView.dialogueTriggerCombatSignal.AddListener(OnDialogueTriggerCombat);
        sceneView.toMapButtonClickedSignal.AddListener(OnToMapButtonClicked);
        sceneView.Init();
    }

    private void OnSceneChange(ESceneChange changeTo) {
        if (changeTo != ESceneChange.VOID) {
            sceneView.InitiateDialogue(scripts[changeTo]);
        }
    }

    private void OnDialogueTriggerCombat(int enemyId) {
        engageCombatSignal.Dispatch(enemyId);
    }

    private void OnToMapButtonClicked() {
        gameFlowStateChangeSignal.Dispatch(EGameFlowState.MAP);
        sceneChangeSignal.Dispatch(ESceneChange.VOID);
    }
}
