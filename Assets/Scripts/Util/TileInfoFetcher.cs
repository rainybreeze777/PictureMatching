using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class TileInfoFetcher {

    private JSONNode spriteJsonSheet = null;
    private static TileInfoFetcher fetcher = null;
    private JSONArray tilesArray = null;

    private List<EElements> validElements = new List<EElements>();

    private TileInfoFetcher () {
        TextAsset jsonText = Resources.Load("SpritesInfo") as TextAsset;
        spriteJsonSheet = JSON.Parse(jsonText.text);
        tilesArray = spriteJsonSheet["tiles"] as JSONArray;

        for (int i = 0; i < tilesArray.Count; ++i) {
            string elemName = tilesArray[i]["name"].ToString().Replace("\"", "");
            try {
                validElements.Add((EElements) Enum.Parse(typeof(EElements), elemName, true));
            } catch (ArgumentException) {
                // Do nothing
            }
        }
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

        string elemName = GetInfoFromNumber(tileNumber, "name");
        EElements eelem = EElements.INVALID;
        try {
            eelem = (EElements) Enum.Parse(typeof(EElements), elemName, true);
        } catch (ArgumentException) {
            // Something went wrong with the conversion
            // Double check elemName returned is spelled correctly
            // and is part of EElements enum
            Debug.LogWarning("Unable to convert from tile number to elements enum. Name from given number is " + elemName);
        }

        return eelem;
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
        return validElements.Count;
    }

    public List<EElements> GetElementsList() {
        return new List<EElements>(validElements);
    }
}
