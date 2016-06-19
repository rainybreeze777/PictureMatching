using System;
using System.Collections.Generic;

public class ComboModel : IComboModel {

    [Inject]
    public ComboPossibleSignal comboPossibleSignal { get; set; }

    private List<int> cancelSequence = new List<int>();
    //List used to track the range of formed combos
    //For example, combo may start from index 2 and end at index 6
    //Then comboTracker will be [2, 6]
    private List<int> comboTracker = new List<int>();

    private ComboListFetcher comboListFetcher;

    private ComboTree comboTree;

    private int numOfTilesOnComboSequence;
    public int NumOfTilesOnComboSequence 
    {
        get { return numOfTilesOnComboSequence; }
        set { numOfTilesOnComboSequence = value; }
    }

    private int comboStart, comboEnd;

    public ComboModel() {
        numOfTilesOnComboSequence = 5;
        comboTree = ComboTree.GetInstance();
        comboListFetcher = ComboListFetcher.GetInstance();
        foreach(List<int> combo in comboListFetcher.GetList()) {
            comboTree.AddCombo(combo, "nameGoesHere");
        }
    }

    public void AddToCancelSequence(int tileNumber) {
        cancelSequence.Add(tileNumber);

        int startIndex = System.Math.Max(0, cancelSequence.Count - numOfTilesOnComboSequence);
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
