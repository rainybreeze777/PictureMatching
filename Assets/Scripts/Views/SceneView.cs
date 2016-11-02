using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;

// Consider using this as dialogue system:
// https://github.com/thesecretlab/YarnSpinner
public class SceneView : View {

    [SerializeField]
    private List<TextAsset> scriptsList;
    private Dictionary<EMapChange, TextAsset> scripts = new Dictionary<EMapChange, TextAsset>();
    private Dictionary<int, Character> charactersMap = new Dictionary<int, Character>();
    [SerializeField] private Text charNameText;
    [SerializeField] private Text dialogueText;

    [SerializeField] private ClickDetector dialogueSystemPanel;

    [Inject]
    public IDialogueParser dialogueParser { get; set; }

    public Signal<int, int> dialogueTriggerCombatSignal = new Signal<int, int>();

    private List<Dialogue> readingDialogue;
    private int lineNumber = 0;
    private int gameSceneId = -1;

    internal void Init() {

        object[] objs = Resources.LoadAll("Characters", typeof(Character));
        for (int i = 0; i < objs.Length; ++i) {
            Character c = (Character) objs[i];
            charactersMap.Add(c.CharId, c);
        }

        // Hand-wire the mapping because Unity3D
        // is too dumb to serialize a dictionary
        // Always make sure the mappings are correct

        scripts.Add(EMapChange.METAL_ARENA, scriptsList[0]);
    
        dialogueSystemPanel.clickSignal.AddListener(OnPanelClicked);
    
        gameObject.SetActive(false);
    }

    public void PrepareScene(EMapChange scene) {
        dialogueParser.ParseDialogue(scripts[scene]);

        gameSceneId = dialogueParser.GetGameSceneId();
    
        List<Dialogue> onEnterDialogues = dialogueParser.GetOnEnterDialogues();
        if (onEnterDialogues.Count > 0) {
            readingDialogue = onEnterDialogues;
            lineNumber = 0;
            gameObject.SetActive(true);
            ReadNextLine();
        }
    }

    private void ReadNextLine() {

        if (readingDialogue == null || readingDialogue.Count == 0) {
            return;
        } 
        if (readingDialogue[lineNumber].IsCombatSignal()) {
            dialogueTriggerCombatSignal.Dispatch(gameSceneId, readingDialogue[lineNumber].GetCombatEnemyId());
            ++lineNumber;
        }
        if (lineNumber >= readingDialogue.Count) {
            readingDialogue.Clear();
            readingDialogue = null;
            lineNumber = 0;
            gameObject.SetActive(false);
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
