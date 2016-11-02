using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IDialogueParser {

    void ParseDialogue(TextAsset dialogueText);
    List<Dialogue> GetOnEnterDialogues();

}
