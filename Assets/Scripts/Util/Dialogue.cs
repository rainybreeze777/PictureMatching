using UnityEngine;
using System.Collections;

public class Dialogue {

    private int characterId = -1;
    private string actorLine = null;

    public Dialogue(int characterId, string actorLine) {
        this.characterId = characterId;
        this.actorLine = actorLine;
    }

    public int GetCharId() { return characterId; }
    public string GetLine() { return actorLine; }

}
