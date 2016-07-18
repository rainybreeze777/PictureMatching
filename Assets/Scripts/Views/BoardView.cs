using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

public class BoardView : View {

    public Signal<Tile> tileSelectedSignal = new Signal<Tile>();
    public Signal<Tile> tileDeselectedSignal = new Signal<Tile>();

    private GameObject boardHolder;
    private const float xOffset = 5.5f;
    private const float yOffset = 0.8f;

    private const string prefabPath = "Prefabs/";
    private TileInfoFetcher infoFetcher;

    //0 = Metal
    //1 = Wood
    //2 = Water
    //3 = Fire
    //4 = Earth
    private List<GameObject> tiles = new List<GameObject>();

    private List<List<GameObject>> onScreenTiles = new List<List<GameObject>>();

    internal void Init(IBoardModel boardModel) {

        infoFetcher = TileInfoFetcher.GetInstance();

        for (int i = 1; i <= infoFetcher.GetTotalNumOfTiles(); ++i) {
           tiles.Add(Resources.Load(prefabPath + infoFetcher.GetInfoFromNumber(i, "prefab")) as GameObject);
        }

        BoardSetup(boardModel);
    }

    public void ResetBoard (IBoardModel boardModel) {
        if (boardHolder != null) {
            Destroy(boardHolder);
        }
        onScreenTiles = new List<List<GameObject>>();

        BoardSetup(boardModel);
    }

    public void DestroyTile(int row, int col) {
        Debug.Log("Destroying tile at row " + (row-1) + " col " + (col-1));
        Destroy(onScreenTiles[row-1][col-1]);
        onScreenTiles[row-1][col-1] = null;
    }

    void BoardSetup (IBoardModel boardModel) {
        boardHolder = new GameObject ("Board");

        int numOfRows = boardModel.numOfRows();
        int numOfColumns = boardModel.numOfColumns();

        // Boolean flag to skip those invalid Tiles that are marked as -1
        bool shouldAddRow = true;
        int onScreenRow = 0;

        for (int r = 0; r < numOfRows; r++) {
            for (int c = 0; c < numOfColumns; c++) {
                int currentTile = boardModel.GetTileAt(r,c);
                if (currentTile != -1 && currentTile != 0) {
                    //the tiles array is used to grab the Array of Tile Sprites in Unity, so it starts from 0
                    //however, the currentTile here is the number represented in Board.
                    //In Board, 0 represents an empty tile, and valid tile index starts from 1.
                    //This system should be changed later, possibly with some tile look up system
                    //in order to avoid confusion.
                    GameObject toInstantiate = tiles[currentTile - 1];

                    GameObject instance = 
                        Instantiate(toInstantiate
                                    , new Vector3(xOffset + c*1.45F, yOffset + r*1.45F, 0f)
                                    , Quaternion.identity)
                        as GameObject;

                    instance.transform.localScale = new Vector3(0.5F, 0.5F, 0);
                    instance.transform.SetParent(boardHolder.transform);
                    
                    if (shouldAddRow) {
                        onScreenTiles.Add(new List<GameObject>());
                        shouldAddRow = false;
                    }
                    onScreenTiles[onScreenRow].Add(instance);

                    //Get the actual Tile Script class in order to call its functions.
                    Tile tile = instance.GetComponent<Tile>();
                    tile.selectSignal.AddListener(tileSelectedSignal.Dispatch);
                    tile.deselectSignal.AddListener(tileDeselectedSignal.Dispatch);
                    //Pass in the currentTile for now, just to be consistent with Board's system of indexing.
                    tile.Initialize(r, c, currentTile);
                }
            }
            if (!shouldAddRow) {
                onScreenRow++;
            }
            shouldAddRow = true;
        }
    }
}
