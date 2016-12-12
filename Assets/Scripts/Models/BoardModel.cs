using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardModel : IBoardModel {

    [Inject]
    public BoardIsEmptySignal boardIsEmptySignal{ get; set; }
    [Inject]
    public TileDestroyedSignal tileDestroyedSignal{ get; set; }
    [Inject]
    public TileRangeDestroyedSignal tileRangeDestroyedSignal{ get; set; }
    [Inject]
    public NewBoardConstructedSignal newBoardConstructedSignal { get; set; }
    [Inject]
    public BattleResultUpdatedSignal battleResultUpdatedSignal { get; set; }
    
    private const int numOfRow = 7;
    private const int numOfColumn = 8;
    private const int numOfSuits = 5;
    
    private int numOfTiles = (numOfRow - 2) * (numOfColumn - 2);
    private int pairs = (numOfRow - 2) * (numOfColumn - 2) / 2;

    private int[,] gameBoard = new int[numOfRow, numOfColumn];
    
    [PostConstruct]
    public void PostConstruct() {
        battleResultUpdatedSignal.AddListener(OnBattleResultUpdated);
    }

    public int numOfRows () {
        return numOfRow;
    }
    
    public int numOfColumns () {
        return numOfColumn;
    }

    public int GetTileAt(int row, int column) {
        if (row < 0 || row >= numOfRow || column < 0 || column >= numOfColumn)
            return -1;
        else
            return gameBoard[row, column];
    }

    public void GenerateBoard() {
        
        RemainingTileGenerator tileGen = new RemainingTileGenerator(numOfRow - 2, numOfColumn - 2);
        numOfTiles = (numOfRow - 2) * (numOfColumn - 2);
        pairs = numOfTiles / 2;
        int remainingPairs = pairs;

        int randomSuit;
        
        while (remainingPairs > 0) {
            Random.InitState((int)System.DateTime.Now.Ticks);
            randomSuit = (int) Random.Range(1, 6); //0 is reserved for empty tile

            Eppy.Tuple<int, int> tileOne;
            Eppy.Tuple<int, int> tileTwo;
            
            do {
                tileOne = tileGen.getRandomFreeTile();
                tileTwo = tileGen.getRandomFreeTile();
            } while(tileOne.Equals(tileTwo));

            gameBoard[tileOne.Item1 + 1 , tileOne.Item2 + 1] = randomSuit;
            gameBoard[tileTwo.Item1 + 1 , tileTwo.Item2 + 1] = randomSuit;
            
            tileGen.removeFromFreeTile(tileOne.Item1, tileOne.Item2);
            tileGen.removeFromFreeTile(tileTwo.Item1, tileTwo.Item2);
            remainingPairs--;
        }

        newBoardConstructedSignal.Dispatch();
    }
    
    public bool isRemovable(int r1, int c1, int r2, int c2) {
        
        //If the tile isn't the same, not removable
        if (gameBoard[r1, c1] != gameBoard[r2, c2]) 
            return false;
        
        //Check for same tile
        if (r1 == r2 && c1 == c2)
            return false;
        
        if(stage1Check(r1, c1, r2, c2))
            return true;
        else if (stage2Check(r1, c1, r2, c2))
            return true;
        else
            return stage3Check(r1, c1, r2, c2);
    }
    
    public void remove(int r, int c) {
        gameBoard[r, c] = 0;

        numOfTiles -= 1;

        tileDestroyedSignal.Dispatch(r, c);

        if (numOfTiles == 0) {
            boardIsEmptySignal.Dispatch();
        }
    }

    public void remove(int r1, int c1, int r2, int c2) {
                
        gameBoard[r1 , c1] = 0;
        gameBoard[r2 , c2] = 0;
        numOfTiles -= 2;

        tileDestroyedSignal.Dispatch(r1, c1);
        tileDestroyedSignal.Dispatch(r2, c2);

        if (numOfTiles == 0) {
            boardIsEmptySignal.Dispatch();
        }
    }
    
    public void removeColumn(int col) {
        for (int r = 0; r < numOfRow; ++r) {
            if (gameBoard[r, col] != 0) {
                gameBoard[r, col] = 0;
                tileDestroyedSignal.Dispatch(r, col);
            }
        }
    }

    // Removing indices are inclusive
    public void removeRange(int startRow, int endRow, int startCol, int endCol)
    {
        for (int r = startRow; r <= endRow; ++r ) {
            for (int c = startCol; c <= endCol; ++c ) {
                if (gameBoard[r, c] != 0) {
                    gameBoard[r, c] = 0;
                }
            }
        }
        tileRangeDestroyedSignal.Dispatch(startRow, endRow, startCol, endCol);
    }

    public bool isEmpty() {
        return numOfTiles == 0;
    }

    private void OnBattleResultUpdated(EBattleResult result) {
        if (result == EBattleResult.UNRESOLVED) {
            GenerateBoard();
        }
    }

    private class RemainingTileGenerator {
        
        List<OneRow> freeTiles = new List<OneRow>();
        
        private class OneRow {
            
            List<int> theColumns = new List<int>();
            int theRowNum;
            
            public OneRow(int rowNum, int columns) {
                
                theRowNum = rowNum;
                
                for (int c = 0; c < columns; c++) {
                    theColumns.Add(c);
                }
            }
            
            public int getThisRowNumber() { return theRowNum; }
            
            public int getRandAvailColumn() { 
                Random.InitState((int)System.DateTime.Now.Ticks);
                
                int randIndex = Random.Range(0, theColumns.Count);
                
                return theColumns[randIndex];
            }
            
            public void removePrevGenCol(int col) {
                
                int index = 0;
                
                foreach (int i in theColumns) {
                    
                    if (i == col) {
                        theColumns.RemoveAt(index);
                        break;
                    }
                    
                    index++;
                }
            }   
            
            public bool isRowAllFilled() {
                return theColumns.Count == 0;
            }
            
        }
        
        public RemainingTileGenerator(int numOfRows, int numOfColumns) {
            
            for (int r = 0; r < numOfRows; r++) {
                OneRow aRow = new OneRow(r, numOfColumns);
                freeTiles.Add(aRow);
            }
            
        }
        
        public Eppy.Tuple<int, int> getRandomFreeTile() {
            
            if (freeTiles.Count == 0){
                return Eppy.Tuple.Create(-1, -1);
            }
            
            int randRowIndex = Random.Range(0, freeTiles.Count);
            OneRow randRow = freeTiles[randRowIndex];
            
            return Eppy.Tuple.Create(randRow.getThisRowNumber(), randRow.getRandAvailColumn());
        }
        
        public void removeFromFreeTile(int row, int column) {
            
            int rowIndex = 0;
            
            foreach (OneRow oneRow in freeTiles) {
                                
                if (oneRow.getThisRowNumber() == row) {
                    oneRow.removePrevGenCol(column);
                    if (oneRow.isRowAllFilled()) {
                        freeTiles.RemoveAt(rowIndex);
                    }
                    
                    break;
                }
                
                rowIndex++;
            }
            
        }
        
    }
    
    private bool stage1Check(int r1, int c1, int r2, int c2) {
        
        //Check for same tile
        if (r1 == r2 && c1 == c2)
            return false;
        
        //Stage 1: Straight line
        if (r1 == r2) {
            for (int i = (c1 < c2 ? c1 : c2) + 1
                    ; i < (c1 < c2 ? c2 : c1)
                    ; i++) {
                //if there is a tile that is not 0, break
                if (gameBoard[r1 , i] != 0)
                    return false;
            }
            
            return true;
        } else if (c1 == c2) {
            for (int i = (r1 < r2 ? r1 : r2) + 1
                    ; i < (r1 < r2 ? r2 : r1)
                    ; i++) {
                //if there is a tile that is not 0, break
                if (gameBoard[i , c1] != 0)
                    return false;
            }
            
            return true;
        }
        
        return false;
    }
    
    private bool stage2Check(int r1, int c1, int r2, int c2) {
        
        //Check for same tile
        if (r1 == r2 && c1 == c2)
            return false;
        
        return (gameBoard[r2 , c1] == 0 && stage1Check(r1, c1, r2, c1) && stage1Check(r2, c1, r2, c2)) 
                || (gameBoard[r1 , c2] == 0 && stage1Check(r1, c1, r1, c2) && stage1Check(r1, c2, r2, c2));
    }
    
    private bool stage3Check(int r1, int c1, int r2, int c2) {
        
        //Check for same tile
        if (r1 == r2 && c1 == c2)
            return false;
        
        //r1 c1 going left 
        for (int c = c1 - 1; c >= 0; c--) {
            if (gameBoard[r1 , c] != 0)
                break;
            else {
                if (stage2Check(r1, c, r2, c2))
                    return true;
            }
        }
        
        //r1 c1 going right
        for (int c = c1 + 1; c < numOfColumn; c++) {
            if (gameBoard[r1 , c] != 0)
                break;
            else {
                if (stage2Check(r1, c, r2, c2))
                    return true;
            }
        }
        
        //r1 c1 going up
        for (int r = r1 - 1; r >= 0; r--) {
            if (gameBoard[r , c1] != 0)
                break;
            else {
                if (stage2Check(r, c1, r2, c2))
                    return true;
            }
        }
        
        //r1 c1 going down
        for (int r = r1 + 1; r < numOfRow; r++) {
            if (gameBoard[r , c1] != 0)
                break;
            else {
                if (stage2Check(r, c1, r2, c2))
                    return true;
            }
        }
        
        //r2 c2 going left 
        for (int c = c2 - 1; c >= 0; c--) {
            if (gameBoard[r2 , c] != 0)
                break;
            else {
                if (stage2Check(r1, c1, r2, c))
                    return true;
            }
        }
        
        //r2 c2 going right
        for (int c = c2 + 1; c < numOfColumn; c++) {
            if (gameBoard[r2 , c] != 0)
                break;
            else {
                if (stage2Check(r1, c1, r2, c))
                    return true;
            }
        }
        
        //r2 c2 going up
        for (int r = r2 - 1; r >= 0; r--) {
            if (gameBoard[r , c2] != 0)
                break;
            else {
                if (stage2Check(r1, c1, r, c2))
                    return true;
            }
        }
        
        //r2 c2 going down
        for (int r = r2 + 1; r < numOfRow; r++) {
            if (gameBoard[r , c2] != 0)
                break;
            else {
                if (stage2Check(r1, c1, r, c2))
                    return true;
            }
        }
        
        return false;
    }
}
