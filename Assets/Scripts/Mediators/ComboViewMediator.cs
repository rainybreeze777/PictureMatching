using System;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

public class ComboViewMediator : Mediator {

    [Inject]
    public ComboView view{ get; set;}
    [Inject]
    public IComboModel comboModel { get; set; }
    [Inject]
    public ResetActiveStateSignal resetActiveStateSignal { get; set; }

    public override void OnRegister() {
        view.Init(comboModel.NumOfTilesOnComboSequence);

        comboModel.CancelAddedSignal.AddListener(OnTileCancelled);
        resetActiveStateSignal.AddListener(ClearCancelSequence);
    }

    public void OnTileCancelled(int tileNumber) {
        view.AddToCancelSequence(tileNumber);
    }

    public void ClearCancelSequence() {
        comboModel.ClearCancelSequence();
        view.ClearOnScreenSequence();
    }
}
