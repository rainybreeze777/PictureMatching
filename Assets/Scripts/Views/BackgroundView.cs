using UnityEngine;
using UnityEngine.UI;
using System;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;

public class BackgroundView : View {

    [Inject]
    public BattleWonSignal battleWonSignal{ get; set;}
    [Inject]
    public BattleLostSignal battleLostSignal{ get; set;}
    [Inject]
    public BattleUnresolvedSignal battleUnresolvedSignal{ get; set;}
    [Inject]
    public InitiateBattleResolutionSignal initiateBattleResolutionSignal { get; set; }
    [Inject]
    public StartGameSignal gameStartSignal { get; set; }

    [SerializeField] private GameObject mainMenuBackground;
    [SerializeField] private GameObject cancellationBackground;

    [PostConstruct]
    public void PostConstruct() {

        battleWonSignal.AddListener(ShowMainMenuBackground);
        battleLostSignal.AddListener(ShowMainMenuBackground);
        battleUnresolvedSignal.AddListener(ShowCancellationBackground);
        initiateBattleResolutionSignal.AddListener(HideAllBackground);
        gameStartSignal.AddListener(ShowCancellationBackground);

        cancellationBackground.SetActive(false);
    }

    private void ShowMainMenuBackground() {
        mainMenuBackground.SetActive(true);
        cancellationBackground.SetActive(false);
    }

    private void ShowCancellationBackground() {
        mainMenuBackground.SetActive(false);
        cancellationBackground.SetActive(true);
    }

    private void HideAllBackground() {
        mainMenuBackground.SetActive(false);
        cancellationBackground.SetActive(false);
    }

}
