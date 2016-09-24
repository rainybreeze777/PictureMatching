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

        for (int i = 0; i < tilesArray.Count; i++) {
            if (tilesArray[i] == null)
                break;
            string name = tilesArray[i]["name"].ToString().Replace("\"", "");
            if (name.Equals(tileName))
                return tilesArray[i]["id"].AsInt;
        }

        return -1;
    }

    public EElements GetElemEnumFromTileNumber(int tileNumber) {
        // TODO: Wire the tile numbers up with actual enums
        // hard-coding for now
        switch (tileNumber) {
            case 1:
                return EElements.METAL;
            case 2:
                return EElements.WOOD;
            case 3:
                return EElements.WATER;
            case 4:
                return EElements.FIRE;
            case 5:
                return EElements.EARTH;
            default:
                return EElements.INVALID;
        }
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

    public int GetTotalNumOfTiles() {
        return tilesArray.Count;
    }

    public int GetTotalNumOfElems() {
        // Hardcode to 5 for now
        return 5;
    }
}
