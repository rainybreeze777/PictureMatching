using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;

// Consider using this as dialogue system:
// https://github.com/thesecretlab/YarnSpinner
public class SceneView : View {

    private Dictionary<int, Character> charactersMap = new Dictionary<int, Character>();
    [SerializeField] private Text charNameText;
    [SerializeField] private Text dialogueText;

    [SerializeField] private GameObject dialogueSystemPanel;
    [SerializeField] private GameObject interestPointsPanel;

    [SerializeField] private Button sceneButton;
    private Dictionary<Button, int> charButtons = new Dictionary<Button, int>();

    [Inject]
    public EnableSceneAfterVictorySignal enableSceneAfterVictorySignal { get; set; }

    public Signal<int> dialogueTriggerCombatSignal = new Signal<int>();
    public Signal<int> charButtonClickedSignal = new Signal<int>();
    public Signal toMapButtonClickedSignal = new Signal();
    public Signal endConversationSignal = new Signal();

    private List<Dialogue> readingDialogue;
    private int lineNumber = 0;

    internal void Init() {

        object[] objs = Resources.LoadAll("Characters", typeof(Character));
        for (int i = 0; i < objs.Length; ++i) {
            Character c = (Character) objs[i];
            charactersMap.Add(c.CharId, c);
        }
    
        dialogueSystemPanel.GetComponent<ClickDetector>().clickSignal.AddListener(OnPanelClicked);
    
        // Init ToMapButton
        Button toMapButton = Instantiate(sceneButton, interestPointsPanel.transform) as Button;
        toMapButton.name = "ToMapButton";
        toMapButton.GetComponentInChildren<Text>().text = "离开";
        toMapButton.onClick.AddListener(toMapButtonClickedSignal.Dispatch);

        dialogueSystemPanel.SetActive(false);
        interestPointsPanel.SetActive(false);
    }

    public void PrepareScene(List<int> allCharsInScene, List<Dialogue> onEnterDialogues) {
        // Clear the existing buttons in interestPointsPanel
        foreach(KeyValuePair<Button, int> kvp in charButtons) {
            Destroy(kvp.Key.gameObject);
        }
        charButtons.Clear();

        // Build the interest points panel
        foreach(int charId in allCharsInScene) {
            Button newButton = Instantiate(sceneButton, interestPointsPanel.transform) as Button;
            newButton.name = charId + "Button";
            newButton.GetComponentInChildren<Text>().text = charactersMap[charId].DisplayName;
            newButton.onClick.AddListener(() => {
                charButtonClickedSignal.Dispatch(charButtons[newButton]);
            });
            charButtons.Add(newButton, charId);
        }

        // Init on enter dialogues, if they exist.
        if (onEnterDialogues.Count > 0) {
        // if (false) {
            readingDialogue = onEnterDialogues;
            lineNumber = 0;
            dialogueSystemPanel.SetActive(true);
            ReadNextLine();
        } else {
            interestPointsPanel.SetActive(true);
        }
    }

    public void StartConversation(List<Dialogue> charRandomDialogue) {
        readingDialogue = charRandomDialogue;
        lineNumber = 0;
        dialogueSystemPanel.SetActive(true);
        interestPointsPanel.SetActive(false);
        ReadNextLine();
    }

    private void ReadNextLine() {

        if (readingDialogue == null || readingDialogue.Count == 0) {
            return;
        } 
        if (readingDialogue[lineNumber].IsCombatSignal()) {
            dialogueTriggerCombatSignal.Dispatch(readingDialogue[lineNumber].GetCombatEnemyId());
            if (readingDialogue[lineNumber].WillEnableSceneAfterVictory()) {
                enableSceneAfterVictorySignal.Dispatch(readingDialogue[lineNumber].GetEnableSceneIdAfterVictory());
            }
            ++lineNumber;
        }
        if (lineNumber >= readingDialogue.Count) {
            readingDialogue.Clear();
            readingDialogue = null;
            lineNumber = 0;
            dialogueSystemPanel.SetActive(false);
            interestPointsPanel.SetActive(true);
            endConversationSignal.Dispatch();
            return;
        }

        int charId = readingDialogue[lineNumber].GetCharId();
        charNameText.text = charactersMap[charId].DisplayName;
        dialogueText.text = readingDialogue[lineNumber].GetLine();
        ++lineNumber;
    }

    public void OnPanelClicked() {
        ReadNextLine();
    }
}
