using UnityEngine;
using UnityEngine.UI;
using Eppy;
using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

public class ComboView : View {

    //0 = Metal
    //1 = Wood
    //2 = Water
    //3 = Fire
    //4 = Earth
    //5 = Unknown
    private List<GameObject> tiles = new List<GameObject>();
    private Sprite maskTileSprite;

    private List<int> enemySequence;
    private GameObject enemyCancelTile = null;
    private uint maskHashCode;
    private int cancelCount = 0;

    private const float distanceBetweenTile = 2.0f;

    private const float xOffset = 3.0f;
    private const float yOffset = 0.77f;
    private const float enemyYOffSet = 8.0f;
    private const string spritePath = "Sprites/";

    private Transform comboDisplayer;
    private GameObject suggestCancelElem1 = null;
    private GameObject suggestCancelElem2 = null;

    private TileInfoFetcher infoFetcher;
    private const string prefabPath = "Prefabs/ComboPrefabs/";

    internal void Init() {

        infoFetcher = TileInfoFetcher.GetInstance();

        for (int i = 1; i <= infoFetcher.GetTotalNumOfTiles(); ++i) {
            string tilePathName = infoFetcher.GetInfoFromNumber(i, "comboPrefab");
            if (tilePathName != null && !tilePathName.Equals("")) {
                tiles.Add(Resources.Load(prefabPath + tilePathName) as GameObject);
            }
        }

        int unknownTileId = infoFetcher.GetTileNumberFromName("Unknown");
        maskTileSprite = Resources.Load<Sprite>(
                            spritePath 
                            + infoFetcher.GetInfoFromNumber(unknownTileId, "normalSprite"));

        comboDisplayer = new GameObject ("ComboDisplayer").transform;
    }

    public void Reset() {
        cancelCount = 0;
    }

    public void AddToCancelSequence(int tileNumber)
    {
        cancelCount++;
        if (cancelCount < enemySequence.Count) {
            UpdateEnemyCancelTileObject(enemySequence[cancelCount], cancelCount);
        } else {
            Destroy(enemyCancelTile);
            enemyCancelTile = null;
        }
        UpdateCancelSuggestion(cancelCount);
    }

    public void ConstructNewEnemySequence(List<int> enemySeq, uint maskHashCode) {
        if (enemySeq.Count <= 0) {
            return;
        }
        enemySequence = enemySeq;

        this.maskHashCode = maskHashCode;
        UpdateEnemyCancelTileObject(enemySequence[0], 0);
        UpdateCancelSuggestion(0);
    }

    private void UpdateCancelSuggestion(int tileSeqIndex) {
        
        if (suggestCancelElem1 != null) {
            Destroy(suggestCancelElem1);
            suggestCancelElem1 = null;
        }
        if (suggestCancelElem2 != null) {
            Destroy(suggestCancelElem2);
            suggestCancelElem2 = null;
        }

        if (tileSeqIndex < enemySequence.Count && !EnemyTileIsHiddenAtIndex(tileSeqIndex)) {
            Tuple<int, int> suggestedCancels = ElementResolver.GetElemsTrumpOver(enemySequence[tileSeqIndex]);
            
            suggestCancelElem1 = 
                Instantiate(tiles[suggestedCancels.Item1 - 1]
                            , new Vector3( 16.5f, enemyYOffSet-0.5f, 0F)
                            , Quaternion.identity) as GameObject;

            suggestCancelElem2 = 
                Instantiate(tiles[suggestedCancels.Item2 - 1]
                            , new Vector3( 16.5f, enemyYOffSet+1.0f, 0F)
                            , Quaternion.identity) as GameObject;

            suggestCancelElem1.transform.localScale = new Vector3(0.85F, 0.85F, 0);
            suggestCancelElem1.transform.SetParent(comboDisplayer);
            suggestCancelElem2.transform.localScale = new Vector3(0.85F, 0.85F, 0);
            suggestCancelElem2.transform.SetParent(comboDisplayer);
        }
    }

    private void UpdateEnemyCancelTileObject(int tileNumber, int tileSeqIndex) {
        if (enemyCancelTile != null) {
            Destroy(enemyCancelTile);
            enemyCancelTile = null;
        }
        GameObject instance = 
            Instantiate(tiles[tileNumber - 1]
                        , new Vector3( xOffset, enemyYOffSet, 0F)
                        , Quaternion.identity) as GameObject;

        instance.transform.localScale = new Vector3(0.5f, 0.5f, 0);
        instance.transform.SetParent(comboDisplayer);

        if (EnemyTileIsHiddenAtIndex(tileSeqIndex)) {
            instance.GetComponent<SpriteRenderer>().sprite = maskTileSprite;
        }

        enemyCancelTile = instance;
    }

    private bool EnemyTileIsHiddenAtIndex(int tileSeqIndex) {
        uint mask = (uint) Mathf.Pow(2, tileSeqIndex);

        return (maskHashCode & mask) == mask;
    }
}
