using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class EnemyModel : IEnemyModel {

    private List<int> generatedSequence;

    [Inject]
    public EnemySeqGenSignal seqGenSignal { get; set; }
    [Inject]
    public BattleUnresolvedSignal battleUnresolvedSignal { get; set; }

    private uint cancelSeqMask = 0; // Binary hash code stored in int

    [PostConstruct]
    public void PostConstruct() {
        battleUnresolvedSignal.AddListener(GenerateSequence);
    }

    public void GenerateSequence() {
        List<int> seq = new List<int>();
        Random.seed = (int)System.DateTime.Now.Ticks;
        //Suppose Enemy right now can have from 9 to 15 cancellations
        int randomSequenceSize = Random.Range(9, 16);
        for (int i = 0; i < randomSequenceSize; i++) {
            seq.Add( Random.Range(1, 6) ); // 0 is reserved for empty
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

    public uint GetPrevSequenceMask() { return cancelSeqMask; }

}
