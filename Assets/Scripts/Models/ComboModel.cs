﻿using System;
using System.Collections.Generic;
using strange.extensions.signal.impl;

public class ComboModel : IComboModel {

    [Inject]
    public ComboPossibleSignal comboPossibleSignal { get; set; }
    public Signal<int> cancelAddedSignal = new Signal<int>();
    public Signal<int> CancelAddedSignal 
    {
        get { return cancelAddedSignal; }
    }

    private List<int> cancelSequence = new List<int>();
    //List used to track the range of formed combos
    //For example, combo may start from index 2 and end at index 6
    //Then comboTracker will be [2, 6]
    private List<int> comboTracker = new List<int>();

    private ComboTree comboTree;

    private int onScreenNumOfTilesOnComboSequence;
    public int NumOfTilesOnComboSequence 
    {
        get { return onScreenNumOfTilesOnComboSequence; }
        set { onScreenNumOfTilesOnComboSequence = value; }
    }

    private int comboStart, comboEnd;

    public ComboModel() {
        onScreenNumOfTilesOnComboSequence = 5;
        comboTree = ComboTree.GetInstance();       
    }

    public void AddToCancelSequence(int tileNumber) {
        cancelSequence.Add(tileNumber);
        cancelAddedSignal.Dispatch(tileNumber);

        int startIndex = System.Math.Max(0, cancelSequence.Count - onScreenNumOfTilesOnComboSequence);
        //Combo length is at least 2
        for (int i = startIndex; i < cancelSequence.Count - 2; i++ ) {

            List<int> subSequence = cancelSequence.GetRange(i, cancelSequence.Count - i);
            string comboName = comboTree.GetCombo(subSequence);
            if (!comboName.Equals("")) {
                comboPossibleSignal.Dispatch(true);
                comboStart = i;
                comboEnd = cancelSequence.Count - i;
                break;
            }
            comboPossibleSignal.Dispatch(false);
        }
    }

    public List<int> GetCancelSeq() {
        return cancelSequence;
    }

    public void MakeCombo() {
        comboTracker.Add(comboStart);
        comboTracker.Add(comboEnd);
    }

    public void ClearCancelSequence() {
        cancelSequence.Clear();
    }
}
