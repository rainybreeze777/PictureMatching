using System;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using strange.extensions.signal.impl;

public class ComboModel : IComboModel {

    [Inject]
    public ComboPossibleSignal comboPossibleSignal { get; set; }
    [Inject]
    public ElemGatherUpdatedSignal elemGatherUpdatedSignal { get; set; }
    public Signal<int> cancelAddedSignal = new Signal<int>();
    public Signal<int> CancelAddedSignal 
    {
        get { return cancelAddedSignal; }
    }

    private List<int> cancelSequence = new List<int>();
    //List used to track the range of formed combos
    //For example, combo may start from index 2 and end at index 6
    //Then comboTracker will be [2, 6]
    private Dictionary<int, OneCombo> equippedComboList;
    private Dictionary<int, bool> skillPrepStatus;

    private TileInfoFetcher tileInfoFetcher;

    // Dict to store gathered elements amount
    private Dictionary<EElements, int> elemGathered = new Dictionary<EElements, int>();

    public ComboModel() {
        equippedComboList = ComboListFetcher.GetInstance().GetMap();
        skillPrepStatus = new Dictionary<int, bool>();
        foreach(int key in equippedComboList.Keys) {
            skillPrepStatus.Add(key, false);
        }
        tileInfoFetcher = TileInfoFetcher.GetInstance();
        List<EElements> validElems = tileInfoFetcher.GetElementsList();
        foreach(EElements elem in validElems) {
            elemGathered.Add(elem, 0);
        }
    }

    public void AddToCancelSequence(int tileNumber) {
        cancelSequence.Add(tileNumber);
        cancelAddedSignal.Dispatch(tileNumber);

        EElements elem = tileInfoFetcher.GetElemEnumFromTileNumber(tileNumber);
        elemGathered[elem] += 1;

        elemGatherUpdatedSignal.Dispatch(elem, elemGathered[elem]);

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

        List<EElements> elems = new List<EElements>();
        foreach(EElements e in elemGathered.Keys) {
            elems.Add(e);
        }

        foreach (EElements e in elems) {
            Assert.IsTrue(elemGathered[e] >= comboToBeDeducted.GetReqFromEElements(e));
            elemGathered[e] -= comboToBeDeducted.GetReqFromEElements(e);
            elemGatherUpdatedSignal.Dispatch(e, elemGathered[e]);
        }

        RefreshSkillPrepStatus();
    }

    private void RefreshSkillPrepStatus() {
        foreach (KeyValuePair<int, OneCombo> kvp in equippedComboList) {

            Dictionary<EElements, int> comboReq = kvp.Value.ElemRequirement();
            Assert.AreEqual(elemGathered.Count, comboReq.Count, "Total number of types of elements are not suppose to be different!");

            bool prevStatus = skillPrepStatus[kvp.Key];
            bool isEnough = true;
            foreach(KeyValuePair<EElements, int> kvp2 in elemGathered) {
                if (elemGathered[kvp2.Key] < comboReq[kvp2.Key]) {
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
