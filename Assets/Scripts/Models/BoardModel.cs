using System;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System.Collections;
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
    
    [Inject]
    public IPlayerStatus playerStatus { get; set; }

    private const int numOfRow = 7;
    private const int numOfColumn = 8;
    private const int numOfSuits = 5;
    
    private int numOfTiles = (numOfRow - 2) * (numOfColumn - 2);
    private int pairs = (numOfRow - 2) * (numOfColumn - 2) / 2;

    private int[,] gameBoard = new int[numOfRow, numOfColumn];
    
    // Base probability is arbitrarily chosen as 10%
    private float BASE_PROB = 10.0f;
    // Arbitrarily chosen max prob for one elem
    private float ONE_ELEM_MAX_PROB = 30.0f;

    Dictionary<EElements, float> elemProbabilities = new Dictionary<EElements, float>();
    Dictionary<EElements, int> equippedElemTendency = new Dictionary<EElements, int>();
    List<EElements> elemOrder = new List<EElements>();

    [PostConstruct]
    public void PostConstruct() {

        elemOrder.Add(EElements.METAL);
        elemOrder.Add(EElements.WOOD);
        elemOrder.Add(EElements.WATER);
        elemOrder.Add(EElements.FIRE);
        elemOrder.Add(EElements.EARTH);
        int count = 0;
        foreach(EElements e in Enum.GetValues(typeof(EElements))) {
            if (e == EElements.NONE) { continue; }

            elemProbabilities.Add(e, 0.0f);
            equippedElemTendency.Add(e, 0);
            count++;
        }
        Assert.IsTrue(count == 5); // Make sure theres only 5 elements for now

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
        
        List<Weapon> playerEquippedWeapons = playerStatus.GetEquippedWeapons();

        // Probability of 0 -> Metal -> Wood -> Water -> Fire -> Earth -> 100
        // Count the distribution of equipped elems; Reset first
        foreach(EElements e in Enum.GetValues(typeof(EElements))) { 
            if (e == EElements.NONE) { continue; }
            equippedElemTendency[e] = 0;
        }

        // Base weapon to element attraction tendency conversion ratio
        // First attempt to assign the elem prob according to
        // each weapon's tier, 1 tier = 1 weaponElemConversion
        float weaponElemConversion = 1.0f; 

        int totalElemTendencyCount = 0;
        foreach(Weapon w in playerEquippedWeapons) {
            if (w.Elem == EElements.NONE) { return; }
            equippedElemTendency[w.Elem] += w.Tier;
            totalElemTendencyCount += w.Tier;
        }

        if (totalElemTendencyCount > 0) {

            // Record calculated raw ratio according to elems
            Dictionary<EElements, float> intermediateProbs = new Dictionary<EElements, float>();

            // First find out the ratio of equipped weapon elems
            float probToBeDistributed = 100.0f - BASE_PROB * elemOrder.Count; // 5 elements
            
            if (totalElemTendencyCount * weaponElemConversion <= probToBeDistributed) {
                // First take the approach of multiplying each elem's tendency with
                // the initial suggested conversion rate. If this total sum doesn't
                // exceed the available probabilities, take this approach and evenly
                // divide the remaining available probabilities between all probabilities

                float remainingProbPerElem = (probToBeDistributed - totalElemTendencyCount * weaponElemConversion) / equippedElemTendency.Count;
                foreach(EElements e in Enum.GetValues(typeof(EElements))) { 
                    if (e == EElements.NONE) { continue; }
                    intermediateProbs.Add(e, weaponElemConversion * equippedElemTendency[e] + remainingProbPerElem);
                }
            } else {
                // If the first suggested probability distribution method exceeds
                // the total available probability, adjust down the conversion rate
                // so that the total ratio of the elem tendencies fit
                weaponElemConversion = probToBeDistributed / totalElemTendencyCount;
                foreach(EElements e in Enum.GetValues(typeof(EElements))) { 
                    if (e == EElements.NONE) { continue; }
                    intermediateProbs.Add(e, weaponElemConversion * equippedElemTendency[e]);
                }
            }

            // Only allow each elem to reach the max prob; if max prob is exceeded,
            // find out the abundant probabilities, and evenly distribute exceeded prob
            // to elems that didn't exceed the limit yet   
            SmoothenElemProbDist(intermediateProbs);

            Debug.Log("Board Generation Probabilities: ");
            foreach(EElements e in Enum.GetValues(typeof(EElements))) { 
                if (e == EElements.NONE) { continue; }

                elemProbabilities[e] = BASE_PROB + intermediateProbs[e];
                Debug.Log(e + " " + elemProbabilities[e]);
            }
        } else {
            float avgProb = 100.0f / elemOrder.Count;
            foreach(EElements e in Enum.GetValues(typeof(EElements))) { 
                if (e == EElements.NONE) { continue; }
                elemProbabilities[e] = avgProb;
            }
        }

        while (remainingPairs > 0) {
            Random.InitState((int)System.DateTime.Now.Ticks);
            randomSuit = GetRandomTile(); //0 is reserved for empty tile

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

    private void SmoothenElemProbDist(Dictionary<EElements, float> elemProbs) {

        int MAX_ITERATION = 20; // Don't let this loop run more than this many times
        // if this limit is exceeded, something probably went wrong with the code 

        float ONE_ELEM_ALLOWANCE = ONE_ELEM_MAX_PROB - BASE_PROB;

        List<EElements> probAvailElems = new List<EElements>();
        Queue<EElements> maxExceededElems = new Queue<EElements>();

        foreach(EElements e in elemProbs.Keys) {
            if (elemProbs[e] > ONE_ELEM_ALLOWANCE) {
                maxExceededElems.Enqueue(e);
            } else if (elemProbs[e] < ONE_ELEM_ALLOWANCE) {
                probAvailElems.Add(e);
            }
        }

        int iteration = 0;
        while (maxExceededElems.Count > 0) {

            if (iteration >= MAX_ITERATION) {
                throw new Exception("SmoothenElemProbDist has exceeded MAX_ITERATION iterations!");
            }

            EElements elemToBeDistributed = maxExceededElems.Dequeue();

            float splitProb = (elemProbs[elemToBeDistributed] - ONE_ELEM_ALLOWANCE) / probAvailElems.Count;

            elemProbs[elemToBeDistributed] = ONE_ELEM_ALLOWANCE;

            List<EElements> probsNoLongerAvailable = new List<EElements>();
            foreach(EElements e in probAvailElems) {
                elemProbs[e] += splitProb;
                if (elemProbs[e] >= ONE_ELEM_ALLOWANCE) {
                    probsNoLongerAvailable.Add(e);
                    if (elemProbs[e] > ONE_ELEM_ALLOWANCE) {
                        maxExceededElems.Enqueue(e);
                    }
                }
            }
            foreach(EElements e in probsNoLongerAvailable) {
                probAvailElems.Remove(e);
            }
            probsNoLongerAvailable.Clear();

            iteration++;
        }
    }

    private int GetRandomTile() {
        // 0 is reserved for empty
        // Let the order of elements that form up to 100.0f% be
        // Metal, Wood, Water, Fire, Earth
        // Each occupy a percentage. Line the percentage up from 0 to 100.0f%
        // then generate a random float from 0 to 100.0f,
        // and determine which range the float belongs to
        // which corresponds to the random element

        float randomElem = Random.Range(0.0f, 100.0f);
        float upperBoundary = 100.0f;
        float lowerBoundary = 100.0f;
        for (int i = elemOrder.Count - 1; i >= 0; --i) {
            float temp = lowerBoundary;
            lowerBoundary -= elemProbabilities[elemOrder[i]];
            upperBoundary = temp;

            if (randomElem >= lowerBoundary && randomElem <= upperBoundary) {
                return i + 1; // i should corresponds to the elem order of the game
            }
        }

        // Should never reach this code
        string warningMsg = "BoardModel GetRandomTile() code reached a place it shouldn't be in!\n";
        warningMsg += "Generated Random is: " + randomElem + "\n";
        warningMsg += "Final Lower Boundary is: " + lowerBoundary + "\n";
        warningMsg += "Final Upper Boundary is: " + upperBoundary + "\n";
        Debug.LogWarning( warningMsg );
        return 1;
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
