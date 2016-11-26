using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

public class SmeltView : View {

    public const EStatusTab THIS_STATUS_TAB = EStatusTab.SMELT;

    [SerializeField] private InputField metalInput;
    [SerializeField] private InputField woodInput;
    [SerializeField] private InputField waterInput;
    [SerializeField] private InputField fireInput;
    [SerializeField] private InputField earthInput;
    [SerializeField] private Button smeltButton;
    [SerializeField] private GameObject obtainInfoPanel;
    [SerializeField] private Text infoPanelTitleText;
    [SerializeField] private Text weaponInfoText;

    public Signal<List<int>> smeltButtonClickedSignal = new Signal<List<int>>();

    internal void Init() {

        obtainInfoPanel.SetActive(false);

        obtainInfoPanel.GetComponent<ClickDetector>().clickSignal.AddListener(OnObtainInfoPanelClicked);
        smeltButton.GetComponent<Button>().onClick.AddListener(() => {
                OnSmelt();
            });
    }

    public void SmeltObtainedWeapon(Weapon w) {

        infoPanelTitleText.text = "获得武器";
        weaponInfoText.text = w.GetWeaponDesc();

        smeltButton.gameObject.SetActive(false);
        obtainInfoPanel.SetActive(true);
    }

    public void SmeltInsufficientEssence() {

        infoPanelTitleText.text = "炼制失败";
        weaponInfoText.text = "灵气不足";

        smeltButton.gameObject.SetActive(false);
        obtainInfoPanel.SetActive(true);
    }

    private void OnSmelt() {
        List<int> spentEssence = new List<int>();

        spentEssence.Add(Int32.Parse(metalInput.text));
        spentEssence.Add(Int32.Parse(woodInput.text));
        spentEssence.Add(Int32.Parse(waterInput.text));
        spentEssence.Add(Int32.Parse(fireInput.text));
        spentEssence.Add(Int32.Parse(earthInput.text));

        smeltButtonClickedSignal.Dispatch(spentEssence);
    }

    private void OnObtainInfoPanelClicked() {
        smeltButton.gameObject.SetActive(true);
        obtainInfoPanel.SetActive(false);
    }

}
