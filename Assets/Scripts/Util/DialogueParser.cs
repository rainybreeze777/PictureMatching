using System;
using UnityEngine;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class DialogueParser : IDialogueParser {

    private JSONNode parsedText = null;
    private JSONNode parsedDialogue = null;
    private string language = "zh_cn";

    private int gameScene = -1;

    private Dictionary<int, List<List<Dialogue>>> randomDialogues = new Dictionary<int, List<List<Dialogue>>>();

    public void ParseDialogue(TextAsset dialogueText) {

        randomDialogues.Clear();

        try {
            parsedText = JSON.Parse(dialogueText.text);
            gameScene = parsedText["gameScene"].AsInt;
            parsedDialogue = parsedText["dialogue"];
        } catch (Exception ex) {
            Debug.LogError(ex.ToString());
        }

        JSONArray randomDialoguesList = parsedDialogue["random"] as JSONArray;
        foreach (JSONNode node in randomDialoguesList) {

            int charId = node["characterId"].AsInt;
            int combatEnemyId = node["combatEnemyId"].AsInt;
            int enableSceneId = node["enableSceneIdAfterVictory"].AsInt;
            JSONArray randomText = node["texts"] as JSONArray;

            List<List<Dialogue>> possibleDialogues = new List<List<Dialogue>>();

            foreach(JSONNode textNode in randomText) {
                List<Dialogue> dialogues = new List<Dialogue>();

                dialogues.Add(new Dialogue(charId, textNode[language]));
                dialogues.Add(new Dialogue(combatEnemyId, enableSceneId));

                possibleDialogues.Add(dialogues);
            }

            randomDialogues.Add(charId, possibleDialogues);
        }
    }

    public List<Dialogue> GetOnEnterDialogues() {
        List<Dialogue> dialogues = new List<Dialogue>();

        JSONArray onEnterArray = parsedDialogue["onEnter"] as JSONArray;
        if (onEnterArray != null) {
            foreach (JSONNode node in onEnterArray) {

                int charId = node["characterId"].AsInt;
                string text = node["text"][language];
                int combatEnemyId = node["combatEnemyId"].AsInt;
                int enableSceneId = node["enableSceneIdAfterVictory"].AsInt;

                if (charId != 0 && combatEnemyId == 0) {
                    dialogues.Add(new Dialogue(charId, text));
                } else if (charId == 0 && combatEnemyId != 0){
                    dialogues.Add(new Dialogue(combatEnemyId, enableSceneId));
                }
            }
        }

        return dialogues;
    }

    public int GetGameSceneId() { return gameScene; }

    public List<int> GetAllCharsInScene() {
        return new List<int>(randomDialogues.Keys);
    }

    public List<Dialogue> GetRandomDialogueForChar(int charId) {

        List<List<Dialogue>> possibleDialogues = randomDialogues[charId];

        List<Dialogue> randomDialogue = new List<Dialogue>(possibleDialogues[Random.Range(0, possibleDialogues.Count)]);

        if (randomDialogue.Count == 0) {
            Debug.LogError("randomDialogue count is 0????");
        }

        return randomDialogue;
    }
}
