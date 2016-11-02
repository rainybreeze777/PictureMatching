using System;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

public class SceneViewMediator : Mediator {

    [Inject]
    public SceneView sceneView{ get; set;}

    [Inject]
    public MapChangeSignal mapChangeSignal { get; set; }
    [Inject]
    public EngageCombatSignal engageCombatSignal { get; set; }

    public override void OnRegister() {

        mapChangeSignal.AddListener(OnMapChange);
        sceneView.dialogueTriggerCombatSignal.AddListener(OnDialogueTriggerCombat);

        sceneView.Init();
    }

    private void OnMapChange(EMapChange changeTo) {
        sceneView.PrepareScene(changeTo);
    }

    private void OnDialogueTriggerCombat(int gameSceneId, int enemyId) {
        engageCombatSignal.Dispatch();
    }

}
