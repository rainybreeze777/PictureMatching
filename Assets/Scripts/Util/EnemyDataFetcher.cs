using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class EnemyDataFetcher {

    private static EnemyDataFetcher fetcher = null;

    private JSONClass enemyDataJsonSheet = null;

    // Map of enemyId to EnemyData object
    private Dictionary<int, EnemyData> enemies = new Dictionary<int, EnemyData>();

    private EnemyDataFetcher() {

        try {
            enemyDataJsonSheet = JSON.Parse((Resources.Load("EnemyData") as TextAsset).text) as JSONClass;
        } catch (Exception ex) {
            Debug.LogError(ex.ToString());
        }

        foreach(KeyValuePair<string, JSONNode> node in enemyDataJsonSheet ) {
            EnemyData enemy = ScriptableObject.CreateInstance(typeof(EnemyData)) as EnemyData;
            JsonUtility.FromJsonOverwrite(node.Value.ToString(), enemy);
            enemy.SerializeSkillReqAndArgs(node.Value["skillReqAndArgs"] as JSONClass);
            enemies.Add(Int32.Parse(node.Key), enemy);
        }
    }

    public static EnemyDataFetcher GetInstance() {
        if (fetcher == null)
            fetcher = new EnemyDataFetcher();

        return fetcher;
    }

    public EnemyData GetEnemyDataById(int id) {
        EnemyData anEnemy;
        if (enemies.TryGetValue(id, out anEnemy)) {
            return anEnemy;
        } else {
            return null;
        }
    }
}
