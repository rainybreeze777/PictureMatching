using System;
using System.Collections.Generic;
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

    [Inject]
    public IDialogueParser dialogueParser { get; set; }

    private Dictionary<ESceneChange, TextAsset> scripts = new Dictionary<ESceneChange, TextAsset>();

    private int gameSceneId = -1;

    public override void OnRegister() {

        // Hand-wire the mapping because Unity3D
        // is too dumb to serialize a dictionary
        // Always make sure the mappings are correct
        scripts.Add(ESceneChange.METAL_ARENA, Resources.Load("Dialogue/Stage1Text") as TextAsset);

        sceneChangeSignal.AddListener(OnSceneChange);
        sceneView.dialogueTriggerCombatSignal.AddListener(OnDialogueTriggerCombat);
        sceneView.toMapButtonClickedSignal.AddListener(OnToMapButtonClicked);
        sceneView.charButtonClickedSignal.AddListener(OnCharButtonClicked);

        sceneView.Init();
    }

    private void OnSceneChange(ESceneChange changeTo) {

        dialogueParser.ParseDialogue(scripts[changeTo]);

        gameSceneId = dialogueParser.GetGameSceneId();

        sceneView.PrepareScene(dialogueParser.GetAllCharsInScene(), dialogueParser.GetOnEnterDialogues());
    }

    private void OnDialogueTriggerCombat(int enemyId) {
        engageCombatSignal.Dispatch(enemyId);
    }

    private void OnToMapButtonClicked() {
        gameFlowStateChangeSignal.Dispatch(EGameFlowState.MAP);
    }

    private void OnCharButtonClicked(int charId) {
        sceneView.StartConversation(dialogueParser.GetRandomDialogueForChar(charId));
    }

}
