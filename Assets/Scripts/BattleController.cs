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

	private List<int> playerSeq;
	private List<int> enemySeq;
	private int resolvingIndex = 0;

	private Transform battleResolveContainer;
	private Camera mainCam = null;

	private float widthSegment = 1F / 10F ;
	private float heightSegment = 1F / 7F ;

	//Update Function Control Flags
	private bool shouldMove = false;
	private bool initiateMove = false;
	private bool shouldResolve = false;

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
		if (shouldResolve) {
			//Do resolution here

			int playerMove = (resolvingIndex < playerSeq.Count) ? playerSeq[resolvingIndex] : -1;
			int enemyMove = (resolvingIndex < enemySeq.Count) ? enemySeq[resolvingIndex] : -1;
			
			if (playerMove != -1 || enemyMove != -1) {
				int compareResult = ResolveAttack(playerMove, enemyMove);
				switch (compareResult) {

					case 0:
						Debug.Log("Ties round " + (resolvingIndex + 1));
						break;
					case 1:
						Debug.Log("Player wins round " + (resolvingIndex + 1 ));
						break;
					case 2:
						Debug.Log("Enemy wins round " + (resolvingIndex + 1));
						break;
					case -1:
						Debug.LogError("ResolveAttack got Invalid Parameters!");
						break;
					default:
						Debug.LogError("Unrecognized result!");
						break;
				}

				resolvingIndex++;
				initiateMove = true;
			}

			shouldResolve = false;
		}
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
			if (percentComplete > 1) {
				shouldMove = false;
				shouldResolve = true;
			}
		}
	}

	//Returns 0 if tie, 1 if tile1 wins, 2 if tile2 wins
	//-1 if parameters are invalid
	//Parameters: tileNumbers as specified in SpritesInfo.json
	//-1 if tile is empty
	private int ResolveAttack(int tile1, int tile2) {

		if (tile1 == -1 && tile2 == -1)
			return 0; //Both are empty Case

		int result = 0;
		Tiles? firstTile = null;
		Tiles? secondTile = null;
		try {
			if (tile1 != -1)
				firstTile = numToTileMap[tile1];
			if (tile2 != -1)
				secondTile = numToTileMap[tile2];
		} catch (KeyNotFoundException) {
			return -1;
		}

		//Check if there are empty tiles
		//One valid tile wins if the other tile is empty
		if (firstTile == null && secondTile != null) 
			return 2;
		if (firstTile != null && secondTile == null)
			return 1;

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

	public void InitiateBattleResolution(List<int> seq) {

		playerSeq = seq;
		enemySeq = EnemyCancellationGenerator.GenerateSequence();

		resolvingIndex = 0;
		
		int maxCount = System.Math.Max(playerSeq.Count, enemySeq.Count);

		for (int i = 0; i < maxCount; i++) {

			string prefabPath = "Prefabs/ComboPrefabs/";

			//Generate Player Cancel Sequence
			if (i < playerSeq.Count) {
				GameObject toInstantiatePlayer = Resources.Load(prefabPath + infoFetcher.GetInfoFromNumber(playerSeq[i], "comboPrefab")) as GameObject;

				if (toInstantiatePlayer == null) {
					Debug.LogError("BattleController Error: toInstantiatePlayer is null!");
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
					Debug.LogError("BattleController Error: toInstantiateEnemy is null!");
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
			throw new System.NullReferenceException("BattleController Error: mainCam is null!");

		Vector3 newVec = mainCam.ViewportToWorldPoint(new Vector3(x, y, 10));

		return newVec;
	}
}
