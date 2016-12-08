using System;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

public class SaveLoadViewMediator : Mediator {

    [Inject]
    public SaveLoadView saveLoadView { get; set; }

    [Inject]
    public ISaverLoader saverLoader { get; set; }

    [Inject]
    public OpenSaveLoadViewSignal openSaveLoadViewSignal { get; set; }

    public override void OnRegister() {

        gameObject.SetActive(false);

        saveLoadView.backButtonClickedSignal.AddListener(OnBackButtonClicked);
        openSaveLoadViewSignal.AddListener(OnOpenSaveLoadViewSignal);

        saveLoadView.Init();
    }

    private void OnBackButtonClicked() {
        saveLoadView.CloseView();
    }

    private void OnOpenSaveLoadViewSignal(bool isSaveView) {
        saveLoadView.OpenView(isSaveView, saverLoader.SlotsCount, saverLoader.LoadGameSaveFromDisk());
    }
}
