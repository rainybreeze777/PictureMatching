using UnityEngine;
using System.Collections;

public class Dialogue {

    private int characterId = -1;
    private string actorLine = null;

    private int gameSceneEnemyId = -1;

    public Dialogue(int characterId, string actorLine) {
        this.characterId = characterId;
        this.actorLine = actorLine;
    }

    public Dialogue(int enemyId) {
    	gameSceneEnemyId = enemyId;
    }

    public int GetCharId() { return characterId; }
    public string GetLine() { return actorLine; }
    public bool IsCombatSignal() { return gameSceneEnemyId != -1; }
    public int GetCombatEnemyId() { return gameSceneEnemyId; }
}
