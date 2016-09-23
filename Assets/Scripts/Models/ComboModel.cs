using System;
using UnityEngine;
using UnityEngine.Assertions;
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
    private Dictionary<int, OneCombo> equippedComboList;
    private Dictionary<int, bool> skillPrepStatus;

    private int comboStart, comboEnd;
    private int comboId = -1;

    // Total of 5 Elements:
    // 0: Metal
    // 1: Wood
    // 2: Water
    // 3: Fire
    // 4: Earth
    private int[] elemGathered = new int[5];

    public ComboModel() {
        equippedComboList = ComboListFetcher.GetInstance().GetMap();
        skillPrepStatus = new Dictionary<int, bool>();
        foreach(int key in equippedComboList.Keys) {
            skillPrepStatus.Add(key, false);
        }
    }

    public void AddToCancelSequence(int tileNumber) {
        cancelSequence.Add(tileNumber);
        cancelAddedSignal.Dispatch(tileNumber);

        ++elemGathered[tileNumber - 1];

        RefreshSkillPrepStatus();
    }

    public List<int> GetCancelSeq() {
        return cancelSequence;
    }

    public void ClearCancelSequence() {
        cancelSequence.Clear();
    }

    public void DeductEquippedComboElems(int comboId) {
        Assert.IsTrue(equippedComboList.ContainsKey(comboId));
        OneCombo comboToBeDeducted = equippedComboList[comboId];

        Assert.IsTrue(elemGathered[0] >= comboToBeDeducted.Metal);
        elemGathered[0] -= comboToBeDeducted.Metal;

        Assert.IsTrue(elemGathered[1] >= comboToBeDeducted.Wood);
        elemGathered[1] -= comboToBeDeducted.Wood;

        Assert.IsTrue(elemGathered[2] >= comboToBeDeducted.Water);
        elemGathered[2] -= comboToBeDeducted.Water;

        Assert.IsTrue(elemGathered[3] >= comboToBeDeducted.Fire);
        elemGathered[3] -= comboToBeDeducted.Fire;

        Assert.IsTrue(elemGathered[4] >= comboToBeDeducted.Earth);
        elemGathered[4] -= comboToBeDeducted.Earth;

        RefreshSkillPrepStatus();
    }

    private void RefreshSkillPrepStatus() {
        foreach (KeyValuePair<int, OneCombo> kvp in equippedComboList) {

            List<int> comboReq = kvp.Value.ElemRequirement();
            Assert.AreEqual(elemGathered.Length, comboReq.Count, "Total number of types of elements are not suppose to be different!");

            bool prevStatus = skillPrepStatus[kvp.Key];
            bool isEnough = true;
            for (int i = 0; i < elemGathered.Length; ++i) {
                if (elemGathered[i] < comboReq[i]) {
                    isEnough = false;
                    break;
                }
            }

            if (isEnough != prevStatus) {
                skillPrepStatus[kvp.Key] = isEnough;
                // Notify others that the availability of this combo has changed
                comboPossibleSignal.Dispatch(kvp.Key, isEnough);
            }
        }
    }
}
