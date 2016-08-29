using System;
using UnityEngine;
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

    private int comboStart, comboEnd;
    private int comboId = -1;

    public ComboModel() {
        comboTree = ComboTree.GetInstance();       
    }

    public void AddToCancelSequence(int tileNumber) {
        cancelSequence.Add(tileNumber);
        cancelAddedSignal.Dispatch(tileNumber);

        int startIndex = System.Math.Max(0, cancelSequence.Count - comboTree.LongestComboLength);
        //Combo length is at least 2
        for (int i = startIndex; i < cancelSequence.Count - 2; i++ ) {

            List<int> subSequence = cancelSequence.GetRange(i, cancelSequence.Count - i);
            comboId = comboTree.GetComboId(subSequence);
            if (comboId != -1) {
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

    [PostConstruct]
    public void PostConstruct()
    {
        Debug.LogWarning("ComboModel injection ready");
    }

    public int MakeCombo() {
        comboTracker.Add(comboStart);
        comboTracker.Add(comboEnd);

        return comboId;
    }

    public void ClearCancelSequence() {
        cancelSequence.Clear();
    }
}
