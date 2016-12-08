using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameStateMachine : IGameStateMachine {

    [Inject]
    public GameFlowStateChangeSignal gameFlowStateChangeSignal { get; set; }
    [Inject]
    public SceneChangeSignal sceneChangeSignal { get; set; }

    private EGameFlowState currentState = EGameFlowState.START;
    public EGameFlowState CurrentState { get { return currentState; } }

    private ESceneChange currentScene = ESceneChange.VOID;
    public ESceneChange CurrentScene { get { return currentScene; } }

    [PostConstruct]
    public void PostConstruct() {
        gameFlowStateChangeSignal.AddListener(OnGameFlowStateChange);
    }

    public void InitFromGameSave(GameSave save) {
        currentState = save.GameState;
    }

    private void OnGameFlowStateChange(EGameFlowState flowState) {
        Debug.Log("GameState changed to " + flowState);
        currentState = flowState;
    }

    private void OnSceneChange(ESceneChange changeToScene) {
        Debug.Log("GameScene changed to " + changeToScene);
        currentScene = changeToScene;
    }
}
