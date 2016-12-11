using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

public class BattleStartView : View {

    [SerializeField] private InputField metalInput;
    [SerializeField] private InputField woodInput;
    [SerializeField] private InputField waterInput;
    [SerializeField] private InputField fireInput;
    [SerializeField] private InputField earthInput;
    [SerializeField] private Button startBattleButton;

    [Inject]
    public PrepareForCombatSignal prepareForCombatSignal { get; set; }
    [Inject]
    public EngageCombatSignal engageCombatSignal { get; set; }

    private int engagingEnemyId = -1;

    [PostConstruct]
    public void PostConstruct() {
        prepareForCombatSignal.AddListener(OnPrepareForCombat);
        startBattleButton.onClick.AddListener(OnStartBattleButtonClicked);

        gameObject.SetActive(false);
    }

    private void OnPrepareForCombat(int enemyId) {
        engagingEnemyId = enemyId;

        metalInput.text = "";
        woodInput.text = "";
        waterInput.text = "";
        fireInput.text = "";
        earthInput.text = "";

        gameObject.SetActive(true);
    }

    private void OnStartBattleButtonClicked() {
        
    }

}
