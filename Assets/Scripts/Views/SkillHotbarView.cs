using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

public class SkillHotbarView : View {

    [SerializeField] private GameObject skillHotbarPanel;
    [SerializeField] private Button toInstantiateButton;

    [Inject]
    public UseComboSignal useComboSignal { get; set; }

    private Dictionary<int, Button> comboButtons;
    private Dictionary<Button, int> comboButtonsReverse;
    private ComboListFetcher fetcher;

    internal void Init() {

        fetcher = ComboListFetcher.GetInstance();
        comboButtons = new Dictionary<int, Button>();
        comboButtonsReverse = new Dictionary<Button, int>();
    }

    public void UpdateSkillHotbar(List<int> comboIds) {
        comboButtons.Clear();
        foreach (int id in comboIds) {
            Button newButton = Instantiate(toInstantiateButton, skillHotbarPanel.transform) as Button;
            newButton.name = "Combo" + id + "Button";
            newButton.GetComponentInChildren<Text>().text = fetcher.GetComboChineseNameById(id);
            newButton.interactable = false;
            newButton.onClick.AddListener(() => {
                    useComboSignal.Dispatch(comboButtonsReverse[newButton]);
                });
            comboButtons.Add(id, newButton);
            comboButtonsReverse.Add(newButton, id);
        }
    }

    public void SetComboPrepStatus(int comboId, bool isAvailable) {
        comboButtons[comboId].interactable = isAvailable;
    }
}
