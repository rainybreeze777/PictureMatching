using System;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

public class SkillHotbarViewMediator : Mediator {

    [Inject]
    public SkillHotbarView skillHotbarView{ get; set;}
    [Inject]
    public ComboPossibleSignal comboPossibleSignal { get; set; }
    [Inject]
    public PlayerEquipComboUpdatedSignal equipComboUpdatedSignal { get; set; }
    [Inject(EInBattleStatusType.PLAYER)]
    public IInBattleStatus playerStatus { get; set; }

    public override void OnRegister() {
        
        comboPossibleSignal.AddListener(OnComboPossible);
        equipComboUpdatedSignal.AddListener(OnEquipComboUpdated);

        skillHotbarView.Init();
    }

    private void OnComboPossible(int possibleComboId, bool isNowPossible) {
        skillHotbarView.SetComboPrepStatus(possibleComboId, isNowPossible);
    }

    private void OnEquipComboUpdated() {
        List<int> equippedComboIds = new List<int>(playerStatus.GetEquippedCombos().Keys);
        skillHotbarView.UpdateSkillHotbar(equippedComboIds);
    }
}
