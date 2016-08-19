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
    private List<GameObject> tiles = new List<GameObject>();

    private List<GameObject> onScreenSequence = new List<GameObject>();
    private int onScreenSequenceCount = 0;

    private const float distanceBetweenTile = 2.0f;
    private int numOfTilesOnComboSequence;
    private const float xOffset = 6.5f;
    private const float yOffset = 0.77f;

    private Transform comboDisplayer;

    private TileInfoFetcher infoFetcher;
    private const string prefabPath = "Prefabs/ComboPrefabs/";

    internal void Init(int comboSequenceLength) {

        infoFetcher = TileInfoFetcher.GetInstance();

        for (int i = 1; i <= 5; ++i) {
           tiles.Add(Resources.Load(prefabPath + infoFetcher.GetInfoFromNumber(i, "comboPrefab")) as GameObject);
        }

        numOfTilesOnComboSequence = comboSequenceLength;
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
}
