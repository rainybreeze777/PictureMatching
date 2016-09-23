using System;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

public class SkillHotbarViewMediator : Mediator {

    [Inject]
    public SkillHotbarView skillHotbarView{ get; set;}
    [Inject]
    public ComboPossibleSignal comboPossibleSignal { get; set; }

    public override void OnRegister() {
        
        comboPossibleSignal.AddListener(OnComboPossible);

        skillHotbarView.Init();
    }

    public void OnComboPossible(int possibleComboId, bool isNowPossible) {
        skillHotbarView.SetComboPrepStatus(possibleComboId, isNowPossible);
    }
}
