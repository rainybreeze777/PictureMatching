﻿using System;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using strange.extensions.signal.impl;

public class ComboModel : IComboModel {

    [Inject]
    public ComboPossibleSignal comboPossibleSignal { get; set; }
    [Inject]
    public ComboExecFinishedSignal comboExecFinishedSignal { get; set; }
    [Inject]
    public ElemGatherUpdatedSignal elemGatherUpdatedSignal { get; set; }
    [Inject(EInBattleStatusType.PLAYER)]
    public IInBattleStatus playerStatus { get; set; }
    [Inject]
    public PlayerEquipComboUpdatedSignal playerEquipComboUpdatedSignal { get; set; }
    [Inject]
    public ResetBattleSignal resetBattleSignal{ get; set; }
    public Signal<int> cancelAddedSignal = new Signal<int>();
    public Signal<int> CancelAddedSignal
    {
        get { return cancelAddedSignal; }
    }
    [Inject]
    public BattleResultUpdatedSignal battleResultUpdatedSignal { get; set; }
    [Inject]
    public GameFlowStateChangeSignal gameFlowStateChangeSignal { get; set; }

    [Inject]
    public IGameStateMachine gameStateMachine { get; set; }

    private List<int> cancelSequence = new List<int>();

    private Dictionary<int, OneCombo> equippedComboList;
    private Dictionary<int, bool> skillPrepStatus;

    private TileInfoFetcher tileInfoFetcher;

    // Dict to store gathered elements amount
    private Dictionary<EElements, int> elemGathered = new Dictionary<EElements, int>();

    private int INIT_ELEM_GATHERED_VALUE = 999;

    public ComboModel() {
        skillPrepStatus = new Dictionary<int, bool>();

        tileInfoFetcher = TileInfoFetcher.GetInstance();
        List<EElements> validElems = tileInfoFetcher.GetElementsList();
        foreach(EElements elem in validElems) {
            elemGathered.Add(elem, INIT_ELEM_GATHERED_VALUE);
        }
    }

    [PostConstruct]
    public void PostConstruct() {
        equippedComboList = playerStatus.GetEquippedCombos();
        comboExecFinishedSignal.AddListener(DeductComboElems);
        playerEquipComboUpdatedSignal.AddListener(RefreshEquippedCombo);
        resetBattleSignal.AddListener(ResetBattleStatus);
        battleResultUpdatedSignal.AddListener(OnBattleResultUpdated);
        gameFlowStateChangeSignal.AddListener(OnGameFlowStateChange);
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

    public void ResetBattleStatus() {
        // When iterating through the dictionary with foreach,
        // the values cannot be modified. Therefore taking
        // a differently approach, i.e. creating a list of keys
        // and access the dictionary using the list
        List<EElements> elems = new List<EElements>(elemGathered.Keys);
        foreach(EElements e in elems) {
            elemGathered[e] = INIT_ELEM_GATHERED_VALUE;
            elemGatherUpdatedSignal.Dispatch(e, INIT_ELEM_GATHERED_VALUE);
        }

        RefreshSkillPrepStatus();
    }

    public void DeductComboElems(int comboId) {
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

    public void RefreshEquippedCombo() {
        skillPrepStatus.Clear();
        foreach(int key in equippedComboList.Keys) {
            skillPrepStatus.Add(key, false);
        }
    }

    public void GainSkillElem(List<int> skillElem) {
        // Hard-code to Metal, Wood, Water, Fire, Earth
        // this should be the accepted protocol
        for(int elem = 0; elem < skillElem.Count; ++elem) {
            EElements e = tileInfoFetcher.GetElemEnumFromTileNumber(elem + 1);
            elemGathered[e] += skillElem[elem];
            elemGatherUpdatedSignal.Dispatch(e, elemGathered[e]);
        }
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
                // if the combo is valid in current gameflow state
                if (kvp.Value.ValidState == gameStateMachine.CurrentState) {
                    comboPossibleSignal.Dispatch(kvp.Key, isEnough);
                }
            }
        }
    }

    private void OnBattleResultUpdated(EBattleResult result) {
        if (result != EBattleResult.NULL) {
            cancelSequence.Clear();
        }
    }

    private void OnGameFlowStateChange(EGameFlowState state) {
        if (state == EGameFlowState.CANCELLATION
            || state == EGameFlowState.BATTLE_RESOLUTION) {

            foreach(int comboId in skillPrepStatus.Keys) {
                if (equippedComboList[comboId].ValidState != state) {
                    comboPossibleSignal.Dispatch(comboId, false);
                } else {
                    comboPossibleSignal.Dispatch(comboId, skillPrepStatus[comboId]);
                }
            }
        }
    }

#if DEVELOPMENT_BUILD
    /* Development only methods. These methods are to be
        used with unit-testing classes exclusively! Calling
        these methods by anything other than unit-testing classes
        are strictly undefined and will not be able to compile
        when releasing a production build! */

    public void UnitTesting_SetEquippedCombolist(Dictionary<int, OneCombo> comboList) {
        equippedComboList = comboList;
    }
#endif
}
