﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

public class BoardView : View {

    public Signal<Tile> tileSelectedSignal = new Signal<Tile>();
    public Signal<Tile> tileDeselectedSignal = new Signal<Tile>();
    public Signal inputCancelledSignal = new Signal();

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
    private List<Tile> highlightedTiles = null;
    private List<Tile> tilesToBeHighlighted = null;

    // Flags used to control interactions with the board elements
    private bool highlightingColumn = false;
    private bool highlightingRange = false;

    // Variables associated with interaction controls
    private int prevHoverCol = -1;
    private int prevHoverTileQuadrant = -1;
    private Tile prevHoverTile = null;
    private int rangeDimensionRow;
    private int rangeDimensionCol;

    private int onScreenRowStart = -1;
    public int SelectedAreaModelRowStart {
        // +1 to account for the extra always empty row in BoardModel at the bottom
        get { return onScreenRowStart + 1; }
    }

    private int onScreenRowEnd = -1;
    public int SelectedAreaModelRowEnd {
        // +1 to account for the extra always empty row in BoardModel at the bottom
        // -1 because onScreenRowEnd is exclusive, whereas
        // the BoardModel is looking for an inclusive number
        // Thus leaving it unchanged
        get { return onScreenRowEnd; }
    }

    private int onScreenColStart = -1;
    public int SelectedAreaModelColStart {
        // +1 to account for the extra always empty col in BoardModel at the left
        get { return onScreenColStart + 1; }
    }

    private int onScreenColEnd = -1;
    public int SelectedAreaModelColEnd {
        // +1 to account for the extra always empty col in BoardModel at the left
        // -1 because onScreenColEnd is exclusive, whereas
        // the BoardModel is looking for an inclusive number
        // Thus leaving it unchanged
        get { return onScreenColEnd; }
    }

    internal void Init() {

        infoFetcher = TileInfoFetcher.GetInstance();

        for (int i = 1; i <= infoFetcher.GetTotalNumOfTiles(); ++i) {
            string tilePathName = infoFetcher.GetInfoFromNumber(i, "prefab");
            if (tilePathName != null && !tilePathName.Equals("")) {
                tiles.Add(Resources.Load(prefabPath + tilePathName) as GameObject);
            }
        }
    }

    public void DestroyTile(int row, int col) {
        // -1 for Row and Col because BoardModel has extra tiles around the whole
        // board for cancel checks. Thus it has 1 extra row and 1 extra column
        // at start that the onScreenTiles doesn't record
        Destroy(onScreenTiles[row-1][col-1]);
        onScreenTiles[row-1][col-1] = null;
    }

    public void EnableHighlightArea(int rowDimension, int columnDimension) {
        if (rowDimension < 1) {
            throw new ArgumentOutOfRangeException("BoardView EnableHighlightArea received row dimension request that is smaller than 1");
        } else if (rowDimension > onScreenTiles.Count) {
            throw new ArgumentOutOfRangeException("BoardView EnableHighlightArea received row dimension request that is larger than the board");
        } else if (columnDimension < 1) {
            throw new ArgumentOutOfRangeException("BoardView EnableHighlightArea received column dimension request that is smaller than 1");
        } else if (columnDimension > onScreenTiles[0].Count) {
            throw new ArgumentOutOfRangeException("BoardView EnableHighlightArea received column dimension request that is larger than the board");
        }

        rangeDimensionRow = rowDimension;
        rangeDimensionCol = columnDimension;
        highlightingRange = true;
    }

    public void DisableHighlightArea() {
        highlightingRange = false;
    }

    public void EnableHighlightColumn() {
        highlightingColumn = true;
    }

    public void DisableHighlightingColumn() {
        highlightingColumn = false;
    }

    public void ClearHighlightedStatus() {
        highlightedTiles = null;
        tilesToBeHighlighted = null;
        prevHoverCol = -1;
        prevHoverTile = null;
        prevHoverTileQuadrant = -1;
    }

    void Update() {

        // On right mouse click, disable skills
        if (Input.GetMouseButtonDown(1)) {
            if (highlightingColumn) {
                DisableHighlightingColumn();
                DehighlightPrevSelection();
                inputCancelledSignal.Dispatch();
            } else if (highlightingRange) {
                DisableHighlightArea();
                DehighlightPrevSelection();
                inputCancelledSignal.Dispatch();
            }
        }

        if (highlightingColumn) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null) {
                Tile hitTile = hit.transform.GetComponent<Tile>();
                HighlightColumn(hitTile.Column);
            } else {
                DehighlightPrevSelection();
                prevHoverCol = -1;
            }
        } else if (highlightingRange) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null) {
                Tile hitTile = hit.transform.GetComponent<Tile>();
                int hitQuadrant = DetermineTileHitQuadrant(hit.collider.bounds, hit.point);
                HighlightRange(hitTile, hitQuadrant);
                prevHoverTile = hitTile;
                prevHoverTileQuadrant = hitQuadrant;
            } else {
                DehighlightPrevSelection();
                prevHoverTile = null;
                prevHoverTileQuadrant = -1;
            }
        } else {
            prevHoverCol = -1;
        }
    }

    public void BoardSetup(IBoardModel boardModel) {

        if (boardHolder != null) {
            Destroy(boardHolder);
        }
        boardHolder = new GameObject ("Board");
        onScreenTiles = new List<List<GameObject>>();

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

                    // instance.transform.localScale = new Vector3(0.5F, 0.5F, 0);
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

    public void ResetAllSkillRequest() {
        DisableHighlightArea();
        DisableHighlightingColumn();
        DehighlightPrevSelection();
    }

    private void HighlightColumn(int col) {

        if (prevHoverCol != col) {
            if (highlightedTiles != null) {
                foreach (Tile t in highlightedTiles) {
                    t.Dehighlight();
                }
                highlightedTiles.Clear();
            } else {
                highlightedTiles = new List<Tile>();
            }

            for (int i = 0; i < onScreenTiles.Count; ++i) {
                // -1 for the extra starting column in BoardModel
                if (onScreenTiles[i][col-1] != null) {
                    Tile t = onScreenTiles[i][col-1].GetComponent<Tile>();
                    highlightedTiles.Add(t);
                    t.Highlight();
                }
            }

            prevHoverCol = col;
        }
    }

    // quadrant is using Cartesian Coordinate system that splits
    // a cartesian plane into 4 quadrants
    private void HighlightRange(Tile selected, int quadrant = -1) {

        int onScreenTileRow = selected.Row - 1;
        int onScreenTileCol = selected.Column - 1;

        // int numOfRows = onScreenTiles.Count;
        // int numOfColumns = onScreenTiles[0].Count;

        int halfHeight = rangeDimensionRow / 2;
        int halfWidth = rangeDimensionCol / 2;

        if ((rangeDimensionRow % 2 == 1) && (rangeDimensionCol % 2 == 1)) {
            // Easy case where the selection square is based
            // on the actual tile squares

            if (prevHoverTile != null && prevHoverTile.Equals(selected)) {
                return;
            }

            // the end coordinates have +1 because the toggle function end marker
            // is exclusive, thus loop needs 1 extra count to end properly
            onScreenRowStart = onScreenTileRow - halfHeight;
            onScreenRowEnd = onScreenTileRow + halfHeight + 1;
            onScreenColStart = onScreenTileCol - halfWidth;
            onScreenColEnd = onScreenTileCol + halfWidth + 1;

        } else if ((rangeDimensionRow % 2 == 0) && (rangeDimensionCol % 2 == 0)) {
            // Slightly harder case where the imaginary selection square
            // has to depend on which quadrant the cursor currently resides
            Assert.AreNotEqual(-1, quadrant);

            if (prevHoverTile != null && prevHoverTile.Equals(selected) && prevHoverTileQuadrant == quadrant) {
                return;
            } 

            switch (quadrant) {
                case 1:
                    onScreenRowStart = onScreenTileRow - halfHeight + 1;
                    onScreenRowEnd = onScreenTileRow + halfHeight + 1;
                    onScreenColStart = onScreenTileCol - halfWidth + 1;
                    onScreenColEnd = onScreenTileCol + halfWidth + 1;
                    break;
                case 2:
                    onScreenRowStart = onScreenTileRow - halfHeight + 1;
                    onScreenRowEnd = onScreenTileRow + halfHeight + 1;
                    onScreenColStart = onScreenTileCol - halfWidth;
                    onScreenColEnd = onScreenTileCol + halfWidth;
                    break;
                case 3:
                    onScreenRowStart = onScreenTileRow - halfHeight;
                    onScreenRowEnd = onScreenTileRow + halfHeight;
                    onScreenColStart = onScreenTileCol - halfWidth;
                    onScreenColEnd = onScreenTileCol + halfWidth;
                    break;
                case 4:
                    onScreenRowStart = onScreenTileRow - halfHeight;
                    onScreenRowEnd = onScreenTileRow + halfHeight;
                    onScreenColStart = onScreenTileCol - halfWidth + 1;
                    onScreenColEnd = onScreenTileCol + halfWidth + 1;
                    break;
            }

            // HighlightRect(onScreenRowStart, onScreenRowEnd, onScreenColStart, onScreenColEnd);
        } else {
            //TODO: Write for the cases where row and columns are not both
            // odd or even
            Debug.LogError("BoardView received unimplemented dimension range!");
            return;
        }

        HighlightRect(onScreenRowStart, onScreenRowEnd, onScreenColStart, onScreenColEnd);
    }

    // End marker is exclusive
    private void HighlightRect(
          int rowStart
        , int rowEnd
        , int colStart
        , int colEnd)
    {
        int numOfRows = onScreenTiles.Count;
        int numOfColumns = onScreenTiles[0].Count;
        tilesToBeHighlighted = new List<Tile>();

        for (int r = rowStart; r < rowEnd; ++r) {
            
            if (r < 0 || r >= numOfRows) { continue; }

            for (int c = colStart; c < colEnd; ++c) {
                
                if (c < 0 || c >= numOfColumns) { continue; }

                if (onScreenTiles[r][c] != null) {
                    Tile t = onScreenTiles[r][c].GetComponent<Tile>();
                    tilesToBeHighlighted.Add(t);
                    t.Highlight();
                }
            }
        }

        // Dehighlight previously highlighted tiles if they are
        // not in range
        if (highlightedTiles != null) {
            foreach (Tile t in highlightedTiles) {
                if (!tilesToBeHighlighted.Contains(t)) {
                    t.Dehighlight();
                }
            }
        }
        highlightedTiles = tilesToBeHighlighted; // Reassign the reference to the new list
        tilesToBeHighlighted = null; // Set reference to null to indicate highlight is done
    }

    private void DehighlightPrevSelection() {

        if (highlightedTiles != null) {
            foreach(Tile t in highlightedTiles) {
                t.Dehighlight();
            }
            highlightedTiles.Clear();
            highlightedTiles = null;
        }
    }

    private int DetermineTileHitQuadrant(Bounds tileBound, Vector2 hitPoint) {
        if (hitPoint.x > tileBound.center.x) {
            return hitPoint.y > tileBound.center.y ? 1 : 4;
        } else {
            return hitPoint.y > tileBound.center.y ? 2 : 3;
        }
    }
}
