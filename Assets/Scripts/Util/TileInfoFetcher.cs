using UnityEngine;
using System.Collections;
using SimpleJSON;

public class TileInfoFetcher {

    private JSONNode spriteJsonSheet = null;
    private static TileInfoFetcher fetcher = null;
    private JSONArray tilesArray = null;

    private TileInfoFetcher () {
        TextAsset jsonText = Resources.Load("SpritesInfo") as TextAsset;
        spriteJsonSheet = JSON.Parse(jsonText.text);
        tilesArray = spriteJsonSheet["tiles"] as JSONArray;
    }

    public static TileInfoFetcher GetInstance() {
        if (fetcher == null)
            fetcher = new TileInfoFetcher();

        return fetcher;
    }

    public int GetTileNumberFromName(string tileName) {
        /*
        int index = 0;
        while (true) {
            if (tilesArray[index] == null)
                break;
            if (tilesArray[index]["name"].Equals(tileName))
                return tilesArray[index]["id"].AsInt;

            index++;
        }
        */

        for (int i = 0; i < tilesArray.Count; i++) {
            if (tilesArray[i] == null)
                break;
            string name = tilesArray[i]["name"].ToString().Replace("\"", "");
            if (name.Equals(tileName))
                return tilesArray[i]["id"].AsInt;
        }

        return -1;
    }

    public string GetInfoFromNumber(int tileNumber, string valueName) {

        for (int i = 0; i < tilesArray.Count; i++) {
            if (tilesArray[i] == null)
                break;
            if (tilesArray[i]["id"].AsInt == tileNumber)
                return tilesArray[i][valueName];
        }

        return "";
    }
}
