using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleController : MonoBehaviour {

	//Win & Loss Rules
	//金克木 木克土 土克水 水克火 火克金
	//金生水 水生木 木生火 火生土 土生金
	//Metal beats Wood and Earth, Beaten by Fire and Water
	//Wood beats Earth and Water, beaten by Metal and Fire
	//Water beats Fire and Metal, beaten by Earth and Wood
	//Fire beats Metal and Wood, beaten by Water and Earth
	//Earth beats Water and Fire, beaten by Wood and Metal

	private TileInfoFetcher infoFetcher;

	private enum Tiles { Metal, Wood, Water, Fire, Earth };
	private Dictionary<int, Tiles> numToTileMap = new Dictionary<int, Tiles>();

	private Transform battleResolveContainer;
	private Camera mainCam = null;

	private float widthSegment = 1F / 10F ;
	private float heightSegment = 1F / 7F ;
	private bool shouldMove = false;
	private bool initiateMove = false;
	private float startTime;
	private Vector3 startPoint, endPoint;
	private float speed = 5.0f;

	void Awake () {
		infoFetcher = TileInfoFetcher.GetInstance();

		foreach (Tiles tile in System.Enum.GetValues(typeof(Tiles))) {
			int theId = infoFetcher.GetTileNumberFromName(tile.ToString());
			numToTileMap[theId] = tile;
		}

		battleResolveContainer = new GameObject("BattleResolveContainer").transform;
	}

	void Start () {
		mainCam = Camera.main;
		/*
		for (int i = 1; i < 6; i++) {
			GameObject instance = 
				Instantiate(toInstantiate
							, camCoordToWorldCoord((2 + i) * widthSegment, 2 * heightSegment)
							, Quaternion.identity) as GameObject;

			instance.transform.SetParent(battleResolveContainer);
		}
		*/
	}

	void Update () {
		if (initiateMove) {
			startPoint = battleResolveContainer.transform.position;
			endPoint = new Vector3(startPoint.x - camCoordToWorldCoord(widthSegment, 0).x, startPoint.y, startPoint.z);
			startTime = Time.time;
			initiateMove = false;
			shouldMove = true;
		}
		if (shouldMove) {
			float percentComplete = speed * (Time.time - startTime);
			battleResolveContainer.transform.position = Vector3.Lerp(startPoint, endPoint, percentComplete);
			Debug.Log("Moving");
			if (percentComplete > 1)
				shouldMove = false;
		}
	}

	//Returns 0 if tie, 1 if tile1 wins, 2 if tile2 wins
	//-1 if parameters are invalid
	private int ResolveAttack(int tile1, int tile2) {

		int result = 0;
		Tiles? firstTile = null;
		Tiles? secondTile = null;
		try {
			firstTile = numToTileMap[tile1];
			secondTile = numToTileMap[tile2];
		} catch (KeyNotFoundException) {
			return -1;
		}

		if (firstTile == secondTile) {
			return result;
		}

		switch (firstTile) {
			case Tiles.Metal:
				if (secondTile == Tiles.Wood || secondTile == Tiles.Earth)
					result = 1;
				else
					result = 2;
				break;
			case Tiles.Wood:
				if (secondTile == Tiles.Earth || secondTile == Tiles.Water)
					result = 1;
				else
					result = 2;
				break;
			case Tiles.Water:
				if (secondTile == Tiles.Fire || secondTile == Tiles.Metal)
					result = 1;
				else
					result = 2;
				break;
			case Tiles.Fire:
				if (secondTile == Tiles.Metal || secondTile == Tiles.Wood)
					result = 1;
				else
					result = 2;
				break;
			case Tiles.Earth:
				if (secondTile == Tiles.Water || secondTile == Tiles.Fire)
					result = 1;
				else
					result = 2;
				break;
		}

		return result;
	}

	public void InitiateBattleResolution(List<int> playerSeq) {
		List<int> enemySeq = EnemyCancellationGenerator.GenerateSequence();
		
		int maxCount = System.Math.Max(playerSeq.Count, enemySeq.Count);

		for (int i = 0; i < maxCount; i++) {

			string prefabPath = "Prefabs/ComboPrefabs/";
			prefabPath += infoFetcher.GetInfoFromNumber(playerSeq[i], "comboPrefab");
			GameObject toInstantiate = Resources.Load(prefabPath) as GameObject;

			if (toInstantiate == null) {
				Debug.LogError("BattleController Error: toInstantiate is null!");
				break;
			}

			GameObject instance = 
				Instantiate(toInstantiate
							, camCoordToWorldCoord((5 + i) * widthSegment, 2 * heightSegment)
							, Quaternion.identity) as GameObject;

			instance.transform.SetParent(battleResolveContainer);
		}

	}

	private Vector3 camCoordToWorldCoord(float x, float y) {
		if (mainCam == null)
			throw new System.NullReferenceException("BattleController Error: mainCam is null!");

		Vector3 newVec = mainCam.ViewportToWorldPoint(new Vector3(x, y, 10));

		return newVec;
	}
}
