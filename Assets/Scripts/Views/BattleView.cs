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

    [Inject(EInBattleStatusType.PLAYER)]
    public IInBattleStatus playerStatus { get; set; }
    [Inject(EInBattleStatusType.ENEMY)]
    public IInBattleStatus enemyStatus { get; set; }

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
    private bool newBattle = true;

    // Injected Signals
    [Inject]
    public BattleWonSignal battleWonSignal{ get; set;}
    [Inject]
    public BattleLostSignal battleLostSignal{ get; set;}
    [Inject]
    public BattleUnresolvedSignal battleUnresolvedSignal{ get; set;}

    internal void Init() {
        infoFetcher = TileInfoFetcher.GetInstance();

        battleResolveContainer = new GameObject("BattleResolveContainer").transform;
        mainCam = Camera.main;
    }

    void Update () {

        bool gameShouldEnd = false;

        if (shouldResolve) {
            //Do resolution here

            if (resultTile != null) {
                Destroy(resultTile);
                resultTile = null;
            }

            int playerMove = (resolvingIndex < playerSeq.Count) ? playerSeq[resolvingIndex] : -1;
            int enemyMove = (resolvingIndex < enemySeq.Count) ? enemySeq[resolvingIndex] : -1;

            GameObject resultTileToInstantiate = null;

            if (playerMove != -1 || enemyMove != -1) {
                int compareResult = ElementResolver.ResolveAttack(playerMove, enemyMove);
                switch (compareResult) {

                    case 0:
                        Debug.Log("Ties round " + (resolvingIndex + 1));
                        resultTileToInstantiate = Resources.Load(prefabPath + infoFetcher.GetInfoFromNumber(playerSeq[resolvingIndex], "comboPrefab")) as GameObject;
                        break;
                    case 1:
                        Debug.Log("Player wins round " + (resolvingIndex + 1 ));
                        resultTileToInstantiate = Resources.Load(prefabPath + infoFetcher.GetInfoFromNumber(playerSeq[resolvingIndex], "comboPrefab")) as GameObject;
                        enemyStatus.ReceiveDmg(playerStatus.Damage);
                        break;
                    case 2:
                        Debug.Log("Enemy wins round " + (resolvingIndex + 1));
                        resultTileToInstantiate = Resources.Load(prefabPath + infoFetcher.GetInfoFromNumber(enemySeq[resolvingIndex], "comboPrefab")) as GameObject;
                        playerStatus.ReceiveDmg(enemyStatus.Damage);
                        break;
                    case -1:
                        Debug.LogError("ResolveAttack got Invalid Parameters!");
                        break;
                    default:
                        Debug.LogError("Unrecognized result!");
                        break;
                }

                resultTile = 
                    Instantiate(resultTileToInstantiate
                                , camCoordToWorldCoord(5.02f * widthSegment, 3.5f * heightSegment)
                                , Quaternion.identity) as GameObject;
                resultTile.transform.localScale = new Vector3(0.75F, 0.75F, 0);

                resolvingIndex++;
                if (playerStatus.IsDead || enemyStatus.IsDead)
                    gameShouldEnd = true;
                else
                    StartCoroutine(WaitBeforeMoving(0.5f));
            } else {
                gameShouldEnd = true;
            }

            if (gameShouldEnd) {
                if (playerStatus.IsDead && !enemyStatus.IsDead)
                    battleLostSignal.Dispatch();
                else if (!playerStatus.IsDead && enemyStatus.IsDead)
                    battleWonSignal.Dispatch();
                else if (!playerStatus.IsDead && !enemyStatus.IsDead)
                    battleUnresolvedSignal.Dispatch();
            }

            shouldResolve = false;
        }
        if (initiateMove) {
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
                shouldResolve = true;
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
        playerStatus.ResetHealth();
        enemyStatus.ResetHealth();
        newBattle = true;
    }

    public void InitiateBattleResolution(List<int> playerSequence, List<int> enemySequence) {

        playerSeq = playerSequence;
        enemySeq = enemySequence;

        resolvingIndex = 0;
        
        int maxCount = System.Math.Max(playerSeq.Count, enemySeq.Count);

        Debug.LogWarning("playerSeq Count " + playerSeq.Count);
        Debug.LogWarning("enemySeq Count " + enemySeq.Count);

        startPoint = battleResolveContainer.transform.position;
        endPoint = Vector3.zero;
        widthBetweenTwoComboTiles = (camCoordToWorldCoord(widthSegment, 0.0f) - camCoordToWorldCoord(0.0f, 0.0f)).x;

        if (newBattle) {
            playerStatus.ResetHealth();
            enemyStatus.ResetHealth();
            newBattle = false;
        }

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

        shouldResolve = true;
    }

    private Vector3 camCoordToWorldCoord(float x, float y) {
        if (mainCam == null)
            throw new System.NullReferenceException("BattleView Error: mainCam is null!");

        Vector3 newVec = mainCam.ViewportToWorldPoint(new Vector3(x, y, 10));

        return newVec;
    }
}
