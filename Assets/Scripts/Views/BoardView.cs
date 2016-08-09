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

    // Flags used to control interactions with the board elements
    public bool highlightingColumn = false;
    private bool highlightingRange = false;

    // Variables associated with interaction controls
    private int prevHoverCol = -1;
    private int prevHoverRow = -1;
    private int prevHoverTileQuadrant = -1;
    private int currentMouseOverTileQuadrant;
    private int rangeDimensionRow;
    private int rangeDimensionCol;

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

    void Update() {

        if (highlightingColumn) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null) {
                Tile hitTile = hit.transform.GetComponent<Tile>();
                if (hitTile != null && prevHoverCol != hitTile.Column) {
                    if (prevHoverCol != -1) {
                        DehighlightColumn(prevHoverCol);
                    }
                    HighlightColumn(hitTile.Column);
                    prevHoverCol = hitTile.Column;
                }
            } else if (prevHoverCol != -1){
                DehighlightColumn(prevHoverCol);
                prevHoverCol = -1;
            }
        } else if (highlightingRange) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null) {
                Tile hitTile = hit.transform.GetComponent<Tile>();
                
                HighlightRange(hitTile);
                prevHoverRow = hitTile.Row;
                prevHoverCol = hitTile.Column;

            } else if (prevHoverCol != -1 || prevHoverRow != -1) {
                DehighlightPrevSelection();
                prevHoverCol = -1;
                prevHoverRow = -1;
            }
        } else {
            prevHoverCol = -1;
        }

    }

    private void BoardSetup (IBoardModel boardModel) {
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

    private void HighlightColumn(int col) {
        for (int i = 0; i < onScreenTiles.Count; ++i) {
            // -1 for the extra starting column in BoardModel
            if (onScreenTiles[i][col-1] != null) {
                onScreenTiles[i][col-1].GetComponent<Tile>().Highlight();
            }
        }
    }

    private void DehighlightColumn(int col) {
        for (int i = 0; i < onScreenTiles.Count; ++i) {
            // -1 for the extra starting column in BoardModel
            if (onScreenTiles[i][col-1] != null) {
                onScreenTiles[i][col-1].GetComponent<Tile>().Dehighlight();
            }
        }
    }

    private void HighlightRange(Tile selected, int quadrant = -1) {

        int onScreenTileRow = selected.Row - 1;
        int onScreenTileCol = selected.Column - 1;

        int numOfRows = onScreenTiles.Count;
        int numOfColumns = onScreenTiles[0].Count;

        int halfHeight = rangeDimensionRow / 2;
        int halfWidth = rangeDimensionCol / 2;

        if ((rangeDimensionRow % 2 == 1) && (rangeDimensionCol % 2 == 1)) {
            // Easy case where the selection square is based
            // on the actual tile squares

            // the end coordinates have +1 because the toggle function end marker
            // is exclusive, thus loop needs 1 extra count to end properly
            ToggleRectHighlight(true, 
                onScreenTileRow - halfHeight,
                onScreenTileRow + halfHeight + 1,
                onScreenTileCol - halfWidth,
                onScreenTileCol + halfWidth + 1 );

            // Placeholder initializations to bypass compiler's check for
            // usage of uninitialized variables
            int rowLoopStart = -1;
            int rowLoopEnd = -1;
            int colLoopStart = -1;
            int colLoopEnd = -1;
            bool shouldDehighlightRow = false;
            bool shouldDehighlightCol = false;

            // Dehighlight columns when moving
            if ( prevHoverCol != -1 && prevHoverCol < selected.Column ) {
                shouldDehighlightCol = true;
                // When moving to the larger col count
                // first convert the tile into onScreenTile coordinates, i.e. -1
                // then - halfwidth to account for the extra col at the left
                colLoopStart = prevHoverCol - halfWidth - 1;
                colLoopEnd = selected.Column - halfWidth - 1;
            } else if ( prevHoverCol != -1 && prevHoverCol > selected.Column) {
                shouldDehighlightCol = true;
                // When moving to the smaller col count
                // first convert the tile into onScreenTile coordinates, i.e. -1
                // then + halfwidth to account for the extra col at the right
                // finally +1 to get the index of the column to be deleted
                // because the deletion loop is in the reverse order
                // so the indexing needs 1 offset
                colLoopStart = selected.Column + halfWidth;
                colLoopEnd = prevHoverCol + halfWidth;
            }
            if (shouldDehighlightCol) {
                ToggleRectHighlight(false, 0, numOfRows, colLoopStart, colLoopEnd);
            }

            // Dehighlight rows when moving
            if ( prevHoverRow != -1 && prevHoverRow < selected.Row ) {
                shouldDehighlightRow = true;
                // Similar reasoning with col
                rowLoopStart = prevHoverRow - halfHeight - 1;
                rowLoopEnd = selected.Row - halfHeight - 1;
            } else if (prevHoverRow != -1 && prevHoverRow > selected.Row ) {
                shouldDehighlightRow = true;
                // similar reasoning with col
                rowLoopStart = selected.Row + halfHeight;
                rowLoopEnd = prevHoverRow + halfHeight;
            }
            if (shouldDehighlightRow) {
                ToggleRectHighlight(false, rowLoopStart, rowLoopEnd, 0, numOfColumns);
            }
        }
    }

    // End marker is exclusive
    private void ToggleRectHighlight(
        bool shouldHighlight
        , int onScreenRowStart
        , int onScreenRowEnd
        , int onScreenColStart
        , int onScreenColEnd)
    {
        
        int numOfRows = onScreenTiles.Count;
        int numOfColumns = onScreenTiles[0].Count;

        for (int r = onScreenRowStart; r < onScreenRowEnd; ++r) {
            
            if (r < 0 || r >= numOfRows) { continue; }

            for (int c = onScreenColStart; c < onScreenColEnd; ++c) {
                
                if (c < 0 || c >= numOfColumns) { continue; }

                if (onScreenTiles[r][c] != null) {
                    if (shouldHighlight) {
                        onScreenTiles[r][c].GetComponent<Tile>().Highlight();
                    } else {
                        onScreenTiles[r][c].GetComponent<Tile>().Dehighlight();
                    }
                }
            }
        }
    }

    private void DehighlightPrevSelection() {

        int onScreenTileRow = prevHoverRow - 1;
        int onScreenTileCol = prevHoverCol - 1;

        int halfHeight = rangeDimensionRow / 2;
        int halfWidth = rangeDimensionCol / 2;

        if ((rangeDimensionRow % 2 == 1) && (rangeDimensionCol % 2 == 1)) {
            ToggleRectHighlight(false, 
                onScreenTileRow - halfHeight,
                onScreenTileRow + halfHeight + 1,
                onScreenTileCol - halfWidth,
                onScreenTileCol + halfWidth + 1 );
        }
    }
}
