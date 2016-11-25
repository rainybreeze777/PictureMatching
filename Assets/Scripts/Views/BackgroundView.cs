using UnityEngine;
using UnityEngine.UI;
using System;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;

public class BackgroundView : View {

    [Inject]
    public BattleResultUpdatedSignal battleResultUpdatedSignal { get; set; }
    [Inject]
    public InitiateBattleResolutionSignal initiateBattleResolutionSignal { get; set; }
    [Inject]
    public EngageCombatSignal engageCombatSignal { get; set; }

    [SerializeField] private GameObject mainMenuBackground;
    [SerializeField] private GameObject cancellationBackground;

    [PostConstruct]
    public void PostConstruct() {

        battleResultUpdatedSignal.AddListener(OnBattleResultUpdated);
        initiateBattleResolutionSignal.AddListener(HideAllBackground);
        engageCombatSignal.AddListener(ShowCancellationBackground);

        cancellationBackground.SetActive(false);
    }

    private void OnBattleResultUpdated(EBattleResult battleResult) {
        if (battleResult == EBattleResult.WON || battleResult == EBattleResult.LOST) {
            ShowMainMenuBackground();
        } else if (battleResult == EBattleResult.UNRESOLVED) {
            ShowCancellationBackground();
        }
    }

    private void ShowMainMenuBackground() {
        mainMenuBackground.SetActive(true);
        cancellationBackground.SetActive(false);
    }

    private void ShowCancellationBackground() {
        ShowCancellationBackground(-1); // Stub enemyId data
    }

    private void ShowCancellationBackground(int enemyId) {
        mainMenuBackground.SetActive(false);
        cancellationBackground.SetActive(true);
    }

    private void HideAllBackground() {
        mainMenuBackground.SetActive(false);
        cancellationBackground.SetActive(false);
    }

}
