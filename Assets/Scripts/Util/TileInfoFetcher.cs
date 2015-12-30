using UnityEngine;
using System.Collections;
using SimpleJSON;

public class TileInfoFetcher {

	private JSONNode spriteJsonSheet = null;
	private static TileInfoFetcher fetcher = null;

	private TileInfoFetcher () {
		TextAsset jsonText = Resources.Load("SpritesInfo") as TextAsset;
		spriteJsonSheet = JSON.Parse(jsonText.text);
	}

	public static TileInfoFetcher GetInstance() {
		if (fetcher == null)
			fetcher = new TileInfoFetcher();

		return fetcher;
	}

	public int GetTileNumberFromName(string tileName) {
		int index = 0;
		var tilesArray = spriteJsonSheet["tiles"];
		while (true) {
			if (tilesArray[index] == null)
				break;
			if (tilesArray[index]["name"].Equals(tileName))
				return tilesArray[index]["id"].AsInt;

			index++;
		}

		return -1;
	}

	public string GetTileNameFromNumber(int tileNumber) {
		int index = 0;
		var tilesArray = spriteJsonSheet["tiles"];
		while (true) {
			if (tilesArray[index] == null)
				break;
			if (tilesArray[index]["id"].AsInt == tileNumber)
				return tilesArray[index]["name"];

			index++;
		}

		return "";
	}
}
