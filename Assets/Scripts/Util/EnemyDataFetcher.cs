using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EnemyDataFetcher {

    private static EnemyDataFetcher fetcher = null;

    // Map of enemyId to EnemyData object
    private Dictionary<int, EnemyData> enemies = new Dictionary<int, EnemyData>();

    private EnemyDataFetcher() {
        object[] objs = Resources.LoadAll("Enemies", typeof(EnemyData));

        for (int i = 0; i < objs.Length; ++i) {

            EnemyData anEnemy = (EnemyData) objs[i];

            if (enemies.ContainsKey(anEnemy.EnemyId)) {
                string errMsg = "EnemyDataFetcher Init Error: An Enemy with the same ID already exists in the dictionary!\n";
                errMsg += "Duplicating EnemyId: " + anEnemy.EnemyId + "\n";

                Debug.LogError(errMsg);
                continue;
            }

            enemies.Add(anEnemy.EnemyId, anEnemy);
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
