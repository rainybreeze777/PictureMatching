using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IDialogueParser {

    void ParseDialogue(TextAsset dialogueText);
    List<Dialogue> GetOnEnterDialogues();
    int GetGameSceneId();
    List<int> GetAllCharsInScene();
    List<Dialogue> GetRandomDialogueForChar(int charId);
}
