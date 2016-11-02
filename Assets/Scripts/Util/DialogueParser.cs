using System;
using UnityEngine;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;

public class DialogueParser : IDialogueParser {

    private JSONNode parsedText = null;
    private JSONNode parsedDialogue = null;
    private string language = "zh_cn";

    public void ParseDialogue(TextAsset dialogueText) {
        try {
            parsedText = JSON.Parse(dialogueText.text);
            parsedDialogue = parsedText["dialogue"];
        } catch (Exception ex) {
            Debug.LogError(ex.ToString());
        }
    }

    public List<Dialogue> GetOnEnterDialogues() {
        List<Dialogue> dialogues = new List<Dialogue>();

        JSONArray onEnterArray = parsedDialogue["onEnter"] as JSONArray;
        foreach (JSONNode node in onEnterArray) {

            int charId = node["characterId"].AsInt;
            string text = node["text"][language];

            dialogues.Add(new Dialogue(charId, text));
        }

        return dialogues;
    }
}
