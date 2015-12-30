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

	void Awake () {
		infoFetcher = TileInfoFetcher.GetInstance();

		foreach (Tiles tile in System.Enum.GetValues(typeof(Tiles))) {
			int theId = infoFetcher.GetTileNumberFromName(tile.ToString());
			numToTileMap[theId] = tile;
		}
	}

	//Returns 0 if tie, 1 if tile1 wins, 2 if tile2 wins
	//-1 if parameters are invalid
	public int ResolveAttack(int tile1, int tile2) {

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
}
