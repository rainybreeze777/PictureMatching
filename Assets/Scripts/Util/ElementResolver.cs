using System;
using Eppy;
using UnityEngine;
using System.Collections.Generic;

public class ElementResolver {

    private enum Tiles { Metal, Wood, Water, Fire, Earth };
    private static Dictionary<int, Tiles> numToTileMap = new Dictionary<int, Tiles>();
    private static Dictionary<Tiles, int> tileToNumMap = new Dictionary<Tiles, int>();

    private static TileInfoFetcher infoFetcher;

    static ElementResolver() {
        infoFetcher = TileInfoFetcher.GetInstance();

        foreach (Tiles tile in System.Enum.GetValues(typeof(Tiles))) {
            int theId = infoFetcher.GetTileNumberFromName(tile.ToString());
            numToTileMap[theId] = tile;
            tileToNumMap[tile] = theId;
        }

    }

    //Returns 0 if tie, 1 if tile1 wins, 2 if tile2 wins
    //-1 if parameters are invalid
    //Parameters: tileNumbers as specified in SpritesInfo.json
    //-1 if tile is empty
    public static int ResolveAttack(int tile1, int tile2) {

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

    public static Tuple<int, int> GetElemsBeatenBy(int elemTileNumber) {
        Tiles? givenTile = null;

        try {
            if (elemTileNumber != -1)
                givenTile = numToTileMap[elemTileNumber];
        } catch (KeyNotFoundException ex) {
            Debug.LogError("ElementResolver Error: GetElemTrumpOver received a tile number that is not recognized");
            Debug.LogError(ex.ToString());
            return null;
        }

        switch (givenTile) {
            case Tiles.Metal:
                return Tuple.Create(tileToNumMap[Tiles.Wood], tileToNumMap[Tiles.Earth]);
            case Tiles.Wood:
                return Tuple.Create(tileToNumMap[Tiles.Earth], tileToNumMap[Tiles.Water]);
            case Tiles.Water:
                return Tuple.Create(tileToNumMap[Tiles.Fire], tileToNumMap[Tiles.Metal]);
            case Tiles.Fire:
                return Tuple.Create(tileToNumMap[Tiles.Metal], tileToNumMap[Tiles.Wood]);
            case Tiles.Earth:
                return Tuple.Create(tileToNumMap[Tiles.Water], tileToNumMap[Tiles.Fire]);
            default:
                // Should never reach this code
                Debug.LogError("ElementResolver Error: Unimplemented TrumpOver Resolution!");
                return null;
        }
    }

    public static Tuple<int, int> GetElemsTrumpOver(int elemTileNumber) {
        Tiles? givenTile = null;

        try {
            if (elemTileNumber != -1)
                givenTile = numToTileMap[elemTileNumber];
        } catch (KeyNotFoundException ex) {
            Debug.LogError("ElementResolver Error: GetElemBeatenBy received a tile number that is not recognized");
            Debug.LogError(ex.ToString());
            return null;
        }

        switch (givenTile) {
            case Tiles.Metal:
                return Tuple.Create(tileToNumMap[Tiles.Fire], tileToNumMap[Tiles.Water]);
            case Tiles.Wood:
                return Tuple.Create(tileToNumMap[Tiles.Metal], tileToNumMap[Tiles.Fire]);
            case Tiles.Water:
                return Tuple.Create(tileToNumMap[Tiles.Earth], tileToNumMap[Tiles.Wood]);
            case Tiles.Fire:
                return Tuple.Create(tileToNumMap[Tiles.Water], tileToNumMap[Tiles.Earth]);
            case Tiles.Earth:
                return Tuple.Create(tileToNumMap[Tiles.Wood], tileToNumMap[Tiles.Metal]);
            default:
                // Should never reach this code
                Debug.LogError("ElementResolver Error: Unimplemented BeatenBy Resolution!");
                return null;
        }
    }

}
