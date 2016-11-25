using System;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

// EnemyModel that is the enemy counter part of ComboModel, but only for enemies
// This class is meant to store the sequences that enemy will generate for one round
// not to be confused with InBattleEnemyStatus, where it stores the current
// enemy battle status information
public class EnemyModel : IEnemyModel {

    private List<int> generatedSequence;

    [Inject]
    public EnemySeqGenSignal seqGenSignal { get; set; }
    [Inject]
    public BattleResultUpdatedSignal battleResultUpdatedSignal { get; set; }

    private uint cancelSeqMask = 0; // Binary hash code stored in int

    private EnemyData readyEnemy = null;

    Dictionary<EElements, float> elemProbabilities = new Dictionary<EElements, float>();

    [PostConstruct]
    public void PostConstruct() {

        int count = 0;

        foreach(EElements e in Enum.GetValues(typeof(EElements))) {
            if (e == EElements.NONE) { continue; }

            elemProbabilities.Add(e, 20.0f);
            count++;
        }
        Assert.IsTrue(count == 5); // Make sure theres only 5 elements for now

        battleResultUpdatedSignal.AddListener(OnBattleResultUpdated);
    }

    private void OnBattleResultUpdated(EBattleResult battleResult) {
        if (battleResult == EBattleResult.UNRESOLVED) {
            GenerateSequence();
        }
    }

    public void SetUpEnemyData(EnemyData enemy) {
        readyEnemy = enemy;

        float probCounter = 0.0f;

        EElements preferredElem = readyEnemy.PreferredElem;
        elemProbabilities[preferredElem] = 20.0f + Random.Range(0.5f, 4.0f) * readyEnemy.Level;
        probCounter += elemProbabilities[preferredElem];
        Debug.Log("PreferredElem " + preferredElem + " with probability " + elemProbabilities[preferredElem]);
        
        EElements counteringElem = preferredElem.ProducedBy();
        elemProbabilities[counteringElem] = 20.0f + Random.Range(0.0f, 2.0f) * readyEnemy.Level;
        probCounter += elemProbabilities[counteringElem];
        Debug.Log("CounteringElem " + counteringElem + " with probability " + elemProbabilities[counteringElem]);
    
        float remainingFloat = (100.0f - probCounter) / 3.0f; // Should have 3 elements not touched
        float probSumCheck = 0.0f;
        foreach(EElements e in Enum.GetValues(typeof(EElements))) {
            if (e == EElements.NONE) { continue; }
            if (e == preferredElem || e == counteringElem) { 
                probSumCheck += elemProbabilities[e];
            } else {
                probSumCheck += remainingFloat;
                elemProbabilities[e] = remainingFloat; // Equally split the remaining probabilities
            }
        }
        Assert.AreApproximatelyEqual(100.0f, probSumCheck);
    }

    public void GenerateSequence() {
        List<int> seq = new List<int>();
        Random.InitState((int)System.DateTime.Now.Ticks);
        // Suppose the minimum number of tiles the enemy can have is
        // depending on the enemy's level
        int randomSequenceSize = Random.Range((int) Mathf.Min(9 + readyEnemy.Level - 1, 15), 16);
        for (int i = 0; i < randomSequenceSize; i++) {
            seq.Add( GetRandomTile() ); 
            if (Random.value < 0.5) { cancelSeqMask += (uint) Mathf.Pow(2, i); } // 50-50 chance of masked or not masked
            // Think of the encoding as this way:
            // Enemy Combo Seq: Fire <- Water <- Fire <- Metal <- Earth
            // Mask Index:      2^4  <-  2^3  <- 2^2  <- 2^1   <- 2^0
            // Combo Sequence in model grows from right to left
            // whereas on screen it will display from left to right
        }

        generatedSequence = new List<int>(seq);

        seqGenSignal.Dispatch();
    }

    public List<int> GetPrevGeneratedSequence() {
        return generatedSequence;
    }

    public uint GetPrevSequenceMask() { 
#if NO_MASK
        return 0;
#else
        return cancelSeqMask;
#endif
    }

    private int GetRandomTile() {
        // 0 is reserved for empty
        // Let the order of elements that form up to 100.0f% be
        // Metal, Wood, Water, Fire, Earth
        // Each occupy a percentage. Line the percentage up from 0 to 100.0f%
        // then generate a random float from 0 to 100.0f,
        // and determine which range the float belongs to
        // which corresponds to the random element

        List<EElements> elemOrder = new List<EElements>();
        elemOrder.Add(EElements.METAL);
        elemOrder.Add(EElements.WOOD);
        elemOrder.Add(EElements.WATER);
        elemOrder.Add(EElements.FIRE);
        elemOrder.Add(EElements.EARTH);

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
        string warningMsg = "EnemyModel GetRandomTile() code reached a place it shouldn't be in!\n";
        warningMsg += "Generated Random is: " + randomElem + "\n";
        warningMsg += "Final Lower Boundary is: " + lowerBoundary + "\n";
        warningMsg += "Final Upper Boundary is: " + upperBoundary + "\n";
        Debug.LogWarning( warningMsg );
        return 1;
    }

}
