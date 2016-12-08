using System;
using UnityEngine;
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

    public override void OnRegister() {

        sceneChangeSignal.AddListener(OnSceneChange);
        sceneView.dialogueTriggerCombatSignal.AddListener(OnDialogueTriggerCombat);
        sceneView.toMapButtonClickedSignal.AddListener(OnToMapButtonClicked);

        sceneView.Init();
    }

    private void OnSceneChange(ESceneChange changeTo) {
        sceneView.PrepareScene(changeTo);
    }

    private void OnDialogueTriggerCombat(int gameSceneId, int enemyId) {
        engageCombatSignal.Dispatch(enemyId);
    }

    private void OnToMapButtonClicked() {
        gameFlowStateChangeSignal.Dispatch(EGameFlowState.MAP);
    }

}
