using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

public class BattleView : View {

    //Win & Loss Rules
    //金克木 木克土 土克水 水克火 火克金
    //金生水 水生木 木生火 火生土 土生金
    //Metal beats Wood and Earth, Beaten by Fire and Water
    //Wood beats Earth and Water, beaten by Metal and Fire
    //Water beats Fire and Metal, beaten by Earth and Wood
    //Fire beats Metal and Wood, beaten by Water and Earth
    //Earth beats Water and Fire, beaten by Wood and Metal

    private TileInfoFetcher infoFetcher;

    private List<int> playerSeq;
    private List<int> enemySeq;
    private int resolvingIndex = 0;

    private Transform battleResolveContainer;
    private Camera mainCam = null;
    private GameObject resultTile = null;

    private float widthSegment = 1F / 10F ;
    private float heightSegment = 1F / 7F ;
    private const string prefabPath = "Prefabs/ComboPrefabs/";

    //Update Function Control Flags
    private bool shouldMove = false;
    private bool initiateMove = false;
    private bool shouldResolve = false;

    private float startTime;
    private Vector3 startPoint, endPoint;
    private float widthBetweenTwoComboTiles;
    private float speed = 5.0f;

    public Signal moveIsDone = new Signal();

    internal void Init() {
        infoFetcher = TileInfoFetcher.GetInstance();

        battleResolveContainer = new GameObject("BattleResolveContainer").transform;
        mainCam = Camera.main;
    }

    void Update () {

        if (shouldResolve) {
            StartCoroutine(WaitBeforeMoving(0.5f));

            shouldResolve = false;
        }
        if (initiateMove) {
            Debug.LogWarning("Move Initiated!");
            startPoint = battleResolveContainer.transform.position;
            endPoint = new Vector3(startPoint.x - widthBetweenTwoComboTiles, startPoint.y, startPoint.z);
            startTime = Time.time;
            initiateMove = false;
            shouldMove = true;
        }
        if (shouldMove) {
            float percentComplete = speed * (Time.time - startTime);
            battleResolveContainer.transform.position = Vector3.Lerp(startPoint, endPoint, percentComplete);
            if (percentComplete > 1) {
                shouldMove = false;
                moveIsDone.Dispatch();
            }
        }
    }

    IEnumerator WaitBeforeMoving(float seconds) {
        yield return new WaitForSeconds(seconds);
        initiateMove = true;
    }

    public void ResetBattle() {
        if (battleResolveContainer != null)
            Destroy(battleResolveContainer.gameObject);
        battleResolveContainer = new GameObject("BattleResolveContainer").transform;
    }

    public void InitiateBattleResolution(List<int> playerSequence, List<int> enemySequence) {

        playerSeq = playerSequence;
        enemySeq = enemySequence;
        
        int maxCount = System.Math.Max(playerSeq.Count, enemySeq.Count);

        Debug.LogWarning("playerSeq Count " + playerSeq.Count);
        Debug.LogWarning("enemySeq Count " + enemySeq.Count);

        startPoint = battleResolveContainer.transform.position;
        endPoint = Vector3.zero;
        widthBetweenTwoComboTiles = (camCoordToWorldCoord(widthSegment, 0.0f) - camCoordToWorldCoord(0.0f, 0.0f)).x;

        for (int i = 0; i < maxCount; i++) {

            //Generate Player Cancel Sequence
            if (i < playerSeq.Count) {
                GameObject toInstantiatePlayer = Resources.Load(prefabPath + infoFetcher.GetInfoFromNumber(playerSeq[i], "comboPrefab")) as GameObject;

                if (toInstantiatePlayer == null) {
                    Debug.LogError("BattleView Error: toInstantiatePlayer is null!");
                    break;
                }

                GameObject instance = 
                    Instantiate(toInstantiatePlayer
                                , camCoordToWorldCoord((5 + i) * widthSegment, 2 * heightSegment)
                                , Quaternion.identity) as GameObject;

                instance.transform.SetParent(battleResolveContainer);
            }

            //Generate Enemy Cancel Sequence
            if (i < enemySeq.Count) {
                GameObject toInstantiateEnemy = Resources.Load(prefabPath + infoFetcher.GetInfoFromNumber(enemySeq[i], "comboPrefab")) as GameObject;

                if (toInstantiateEnemy == null) {
                    Debug.LogError("BattleView Error: toInstantiateEnemy is null!");
                    break;
                }

                GameObject instance = 
                    Instantiate(toInstantiateEnemy
                                , camCoordToWorldCoord((5 + i) * widthSegment, 5 * heightSegment)
                                , Quaternion.identity) as GameObject;

                instance.transform.SetParent(battleResolveContainer);
            }
        }
    }

    public void UpdateResultTile(int tileNumber) {

        if (resultTile != null) {
            Destroy(resultTile);
            resultTile = null;
        }

        GameObject resultTileToInstantiate = Resources.Load(prefabPath + infoFetcher.GetInfoFromNumber(tileNumber, "comboPrefab")) as GameObject;

        resultTile = 
            Instantiate(resultTileToInstantiate
                        , camCoordToWorldCoord(5.02f * widthSegment, 3.5f * heightSegment)
                        , Quaternion.identity) as GameObject;
        resultTile.transform.localScale = new Vector3(0.75F, 0.75F, 0);

        shouldResolve = true;
    }

    private Vector3 camCoordToWorldCoord(float x, float y) {
        if (mainCam == null)
            throw new System.NullReferenceException("BattleView Error: mainCam is null!");

        Vector3 newVec = mainCam.ViewportToWorldPoint(new Vector3(x, y, 10));

        return newVec;
    }
}
