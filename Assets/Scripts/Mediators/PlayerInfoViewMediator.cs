using System;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

public class PlayerInfoViewMediator : Mediator {

    [Inject]
    public PlayerInfoView infoView { get; set; }

    [Inject]
    public IPlayerStatus playerStatus { get; set; }

    [Inject]
    public PlayerInfoUpdatedSignal playerInfoUpdatedSignal { get; set; }

    public override void OnRegister() {
        infoView.Init();

        playerInfoUpdatedSignal.AddListener(OnPlayerInfoUpdated);

        OnPlayerInfoUpdated(); // Initialize the fields
    }

    private void OnPlayerInfoUpdated() {
        infoView.UpdateHealthText(playerStatus.Health);
        infoView.UpdateDamageText(playerStatus.Damage);
        infoView.UpdateMetalText(playerStatus.MetalEssence);
        infoView.UpdateWoodText(playerStatus.WoodEssence);
        infoView.UpdateWaterText(playerStatus.WaterEssence);
        infoView.UpdateFireText(playerStatus.FireEssence);
        infoView.UpdateEarthText(playerStatus.EarthEssence);
    }
}
