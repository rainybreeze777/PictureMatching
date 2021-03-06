﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;
using Yarn.Unity;

// Consider using this as dialogue system:
// https://github.com/thesecretlab/YarnSpinner
public class SceneView : View {

    private Dictionary<int, Character> charactersMap = new Dictionary<int, Character>();
    [SerializeField] private Text charNameText;
    [SerializeField] private Text dialogueText;

    [SerializeField] private GameObject dialogueSystemPanel;
    [SerializeField] private GameObject interestPointsPanel;
    [SerializeField] private DialogueRunner dialogueRunner;

    [SerializeField] private Button sceneButton;
    private Dictionary<Button, int> charButtons = new Dictionary<Button, int>();

    [Inject]
    public BattleResultUpdatedSignal battleResultUpdatedSignal { get; set; }

    public Signal<int> dialogueTriggerCombatSignal = new Signal<int>();
    public Signal<int> charButtonClickedSignal = new Signal<int>();
    public Signal toMapButtonClickedSignal = new Signal();
    // public Signal endConversationSignal = new Signal();

    private int lineNumber = 0;

    private string YARN_LOCATION_NODE_SUFFIX = ".locations";
    private string previousScriptName;

    internal void Init() {

        object[] objs = Resources.LoadAll("Characters", typeof(Character));
        for (int i = 0; i < objs.Length; ++i) {
            Character c = (Character) objs[i];
            charactersMap.Add(c.CharId, c);
        }


        // Init ToMapButton
        Button toMapButton = Instantiate(sceneButton, interestPointsPanel.transform) as Button;
        toMapButton.name = "ToMapButton";
        toMapButton.GetComponentInChildren<Text>().text = "离开";
        toMapButton.onClick.AddListener(toMapButtonClickedSignal.Dispatch);

        dialogueSystemPanel.SetActive(false);
        interestPointsPanel.SetActive(false);

        DialogueSystemUI systemUI = dialogueSystemPanel.GetComponent<DialogueSystemUI>();
        dialogueSystemPanel.GetComponent<ClickDetector>().clickSignal.AddListener(() => {
            systemUI.ReadNextLine();
        });
        systemUI.AddLineObserver(DisplayLine);
        systemUI.AddInteractTargetObserver(DisplayInteractTargets);
        systemUI.AddOptionChosenObserver(OnOptionChosen);
        systemUI.AddConvoCompleteObserver(OnConvoComplete);
        systemUI.AddInitCombatObserver(OnTriggerCombat);
        systemUI.AddDialogueCompleteObserver(OnDialogueComplete);

        battleResultUpdatedSignal.AddListener(OnBattleResultUpdated);
    }

    public void InitiateDialogue(string scriptName) {
        previousScriptName = scriptName;
        dialogueRunner.StartDialogue(scriptName);
    }

    public void DisplayLine(int speakerId, string line) {

        dialogueSystemPanel.SetActive(true);

        charNameText.text = charactersMap[speakerId].DisplayName;
        dialogueText.text = line;
    }

    public void DisplayInteractTargets(List<string> targets) {
        // Clear the existing buttons in interestPointsPanel
        foreach(KeyValuePair<Button, int> kvp in charButtons) {
            Destroy(kvp.Key.gameObject);
        }
        charButtons.Clear();

        // Build the interest points panel
        int index = 0;
        foreach(string targ in targets) {
            Button newButton = Instantiate(sceneButton, interestPointsPanel.transform) as Button;
            newButton.name = targ + "Button";
            newButton.GetComponentInChildren<Text>().text = targ;
            newButton.onClick.AddListener(() => {
                dialogueSystemPanel.GetComponent<DialogueSystemUI>().SelectOption(charButtons[newButton]);
            });
            charButtons.Add(newButton, index);
            index++;
        }

        interestPointsPanel.SetActive(true);
    }

    public void OnOptionChosen(int chosenOption) {
        // parameter can be ignored
        interestPointsPanel.SetActive(false);
    }

    public void OnConvoComplete(string nextNode) {
        dialogueSystemPanel.SetActive(false);
    }

    public void OnTriggerCombat(int enemyId) {
        dialogueTriggerCombatSignal.Dispatch(enemyId);
    }

    public void OnBattleResultUpdated(EBattleResult result) {
        dialogueSystemPanel.GetComponent<DialogueSystemUI>().SetBattleComplete();
    }

    public void OnDialogueComplete() {
        dialogueRunner.StartDialogue(previousScriptName + YARN_LOCATION_NODE_SUFFIX);
    }
}
