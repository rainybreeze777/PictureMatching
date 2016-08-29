using UnityEngine;
using UnityEngine.UI;
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

    private List<GameObject> onScreenSequence = new List<GameObject>();
    private List<GameObject> onScreenEnemySequence = new List<GameObject>();
    private int onScreenSequenceCount = 0;

    private const float distanceBetweenTile = 2.0f;
    private const int numOfTilesOnComboSequence = 5;

    private const float xOffset = 6.5f;
    private const float yOffset = 0.77f;
    private const float enemyYOffSet = 9.0f;
    private const string spritePath = "Sprites/";

    private Transform comboDisplayer;

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

    public void ClearOnScreenSequence() {
        foreach(GameObject obj in onScreenSequence)
            Destroy(obj);
        onScreenSequence.Clear();
        onScreenSequenceCount = 0;
    }

    public void AddToCancelSequence(int tileNumber)
    {
        GameObject toInstantiate = tiles[tileNumber - 1];

        if (onScreenSequenceCount >= numOfTilesOnComboSequence) {
            GameObject headTile = onScreenSequence [0];
            onScreenSequence.RemoveAt (0);
            Destroy (headTile);

            foreach (GameObject aTile in onScreenSequence) {
                //For now there is no animations
                //Animation of movement needs some investigation
                aTile.transform.Translate (Vector3.left * distanceBetweenTile);
            }
        } else {
            onScreenSequenceCount++;
        }

        GameObject instance = 
            Instantiate(toInstantiate
                        , new Vector3( xOffset + (onScreenSequenceCount - 1) * distanceBetweenTile, yOffset, 0F)
                        , Quaternion.identity) as GameObject;

        instance.transform.localScale = new Vector3(0.5F, 0.5F, 0);
        instance.transform.SetParent(comboDisplayer);

        onScreenSequence.Add(instance);
    }

    public void ConstructNewEnemySequence(List<int> enemySeq, int maskHashCode) {
        onScreenEnemySequence.Clear();

        for (int i = 0; i < numOfTilesOnComboSequence; ++i) {

            int mask = (int) Mathf.Pow(2, numOfTilesOnComboSequence - 1 - i);

            GameObject instance = 
                Instantiate(tiles[enemySeq[i] - 1]
                            , new Vector3( xOffset + i * distanceBetweenTile, enemyYOffSet, 0F)
                            , Quaternion.identity) as GameObject;

            instance.transform.localScale = new Vector3(0.5f, 0.5f, 0);
            instance.transform.SetParent(comboDisplayer);

            if ((maskHashCode & mask) == mask) {
                instance.GetComponent<SpriteRenderer>().sprite = maskTileSprite;
            }

            onScreenEnemySequence.Add(instance);
        }
    }
}
