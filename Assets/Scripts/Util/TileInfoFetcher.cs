using UnityEngine;
using System.Collections;
using SimpleJSON;

public class TileInfoFetcher {

	private string spriteJsonSheet = null;
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
}
