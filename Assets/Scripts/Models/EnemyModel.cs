﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class EnemyModel : IEnemyModel {

    private List<int> generatedSequence;

    [Inject]
    public EnemySeqGenSignal seqGenSignal { get; set; }
    [Inject]
    public BattleUnresolvedSignal battleUnresolvedSignal { get; set; }

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
        }

        generatedSequence = new List<int>(seq);

        seqGenSignal.Dispatch();
    }

    public List<int> GetPrevGeneratedSequence() {
        return new List<int>(generatedSequence);
    }

}
