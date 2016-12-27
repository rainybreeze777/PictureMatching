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
    public SceneLoadFromSaveSignal sceneLoadFromSaveSignal { get; set; }
    [Inject]
    public EngageCombatSignal engageCombatSignal { get; set; }
    [Inject]
    public GameFlowStateChangeSignal gameFlowStateChangeSignal { get; set; }

    [Inject]
    public IDialogueParser dialogueParser { get; set; }
    [Inject]
    public IBiographer playerBiographer { get; set; }

    private Dictionary<ESceneChange, TextAsset> scripts = new Dictionary<ESceneChange, TextAsset>();

    private int gameSceneId = -1;

    public override void OnRegister() {

        // Hand-wire the mapping because Unity3D
        // is too dumb to serialize a dictionary
        // Always make sure the mappings are correct
        scripts.Add(ESceneChange.STAGE1, Resources.Load("Dialogue/Stage1Text") as TextAsset);
        scripts.Add(ESceneChange.STAGE2, Resources.Load("Dialogue/Stage2Text") as TextAsset);

        sceneChangeSignal.AddListener(OnSceneChange);
        sceneLoadFromSaveSignal.AddListener(OnDirectLoadScene);
        sceneView.dialogueTriggerCombatSignal.AddListener(OnDialogueTriggerCombat);
        sceneView.toMapButtonClickedSignal.AddListener(OnToMapButtonClicked);
        sceneView.charButtonClickedSignal.AddListener(OnCharButtonClicked);
        sceneView.endConversationSignal.AddListener(OnEndConversation);

        sceneView.Init();
    }

    private void OnSceneChange(ESceneChange changeTo) {

        if (changeTo != ESceneChange.VOID) {
            dialogueParser.ParseDialogue(scripts[changeTo]);
            gameSceneId = dialogueParser.GetGameSceneId();
            Assert.IsTrue(playerBiographer.IsAtMap()); // Integrity check

            List<Dialogue> onEnterDialogues;
            if (playerBiographer.AlreadyVisitedFromCurrentPoint(gameSceneId)) {
                onEnterDialogues = new List<Dialogue>();
            } else {
                onEnterDialogues = dialogueParser.GetOnEnterDialogues();
            }
            sceneView.PrepareScene(dialogueParser.GetAllCharsInScene(), onEnterDialogues);

            playerBiographer.Visit(gameSceneId);
        }
    }

    private void OnDialogueTriggerCombat(int enemyId) {
        engageCombatSignal.Dispatch(enemyId);
    }

    private void OnToMapButtonClicked() {
        playerBiographer.Leave();
        Assert.IsTrue(playerBiographer.IsAtMap()); // Integrity check
        gameFlowStateChangeSignal.Dispatch(EGameFlowState.MAP);
        sceneChangeSignal.Dispatch(ESceneChange.VOID);
    }

    private void OnCharButtonClicked(int charId) {
        if (playerBiographer.AlreadyVisitedFromCurrentPoint(charId)) {
            // TODO: Stud for now
        } else {
            // TODO: Stud for now
        }
        sceneView.StartConversation(dialogueParser.GetRandomDialogueForChar(charId));
        playerBiographer.Visit(charId);
    }

    private void OnEndConversation() {
        playerBiographer.Leave();
    }

    private void OnDirectLoadScene(ESceneChange loadScene) {
        Assert.IsTrue(loadScene != ESceneChange.VOID);

        dialogueParser.ParseDialogue(scripts[loadScene]);
        gameSceneId = dialogueParser.GetGameSceneId();
        // For now there are no nested scenes implemented, so dialogue parser
        // won't be able to handle scenes that have nested scenes
        sceneView.PrepareScene(dialogueParser.GetAllCharsInScene(), new List<Dialogue>());
    }
}
