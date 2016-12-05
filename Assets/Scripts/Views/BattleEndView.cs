using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;

public class BattleEndView : View, IPointerClickHandler {

    [Inject]
    public BattleResultUpdatedSignal battleResultUpdatedSignal { get; set; }
    [Inject]
    public PlayerEssenceGainedSignal playerEssenceGainedSignal { get; set; }
    [Inject]
    public GameFlowStateChangeSignal gameFlowStateChangeSignal { get; set; }

    public Signal clickedSignal = new Signal();

    [SerializeField] private Text titleText;
    [SerializeField] private Text bootyText;

    [PostConstruct]
    public void PostConstruct() {
        battleResultUpdatedSignal.AddListener(OnBattleResultUpdated);
        playerEssenceGainedSignal.AddListener(OnEssenceGained);
    }

    private void OnEssenceGained(List<int> essence) {
        List<string> essenceList = new List<string>() {"金", "木", "水", "火", "土"};
    
        string output = "获得灵气: \n";

        Assert.IsTrue(essence.Count == 5);
        for (int i = 0; i < essence.Count; ++i) {
            output += essenceList[i] + ": " + essence[i] + "\n";
        }

        bootyText.text = output;
    }

    private void OnBattleResultUpdated(EBattleResult battleResult) {
        if (battleResult == EBattleResult.WON) {
            titleText.text = "胜利!";
        } else if (battleResult == EBattleResult.LOST) {
            titleText.text = "失败...";
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        clickedSignal.Dispatch();
    }
}
