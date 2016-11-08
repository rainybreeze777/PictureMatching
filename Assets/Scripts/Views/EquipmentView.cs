using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

public class EquipmentView : View {

    [SerializeField] private Button confirmEquipButton;
    [SerializeField] private GameObject weaponEquipPanel;

    [Inject]
    public StatusTabChangedSignal statusTabChangedSignal { get; set; }
    [Inject]
    public GameFlowStateChangeSignal gameFlowStateChangeSignal { get; set; }

    public Signal confirmEquipSignal = new Signal();
    private List<Weapon> weaponsInPossession = null;
    private List<Weapon> equippedWeapons = new List<Weapon>();
    private List<Toggle> selections = new List<Toggle>();

    public const EStatusTab THIS_STATUS_TAB = EStatusTab.EQUIP;
    private const int EQUIP_NUM_MAX = 3;
    private int numOfEquipped = 0;

    private GameObject toInstantiate = null;

    internal void Init() {

        toInstantiate = Resources.Load("Prefabs/UI/Toggle") as GameObject;

        statusTabChangedSignal.AddListener(OnStatusTabChanged);
        gameFlowStateChangeSignal.AddListener(OnGameFlowStateChange);

        confirmEquipButton.GetComponent<Button>().onClick.AddListener(OnConfirmEquipButtonClicked);
        confirmEquipButton.interactable = false;

        gameObject.SetActive(false);
    }

    public List<Weapon> GetEquippedWeapons() {
        return new List<Weapon>(equippedWeapons);
    }

    public void RefreshEquipmentView(List<Weapon> weapons, List<Weapon> alreadyEquippedWeapons) {

        foreach (Toggle t in selections) {
            Destroy(t.gameObject);
        }
        selections.Clear();

        foreach (Weapon w in weapons) {
            Toggle newToggle = (Toggle.Instantiate(toInstantiate, weaponEquipPanel.transform) as GameObject).GetComponent<Toggle>();
            newToggle.name = w.GetWeaponName() + "Toggle";
            newToggle.GetComponentInChildren<Text>().text = w.GetWeaponDesc();
            newToggle.isOn = alreadyEquippedWeapons.Contains(w);
            newToggle.onValueChanged.AddListener(OnToggleValueChange);
            selections.Add(newToggle);
        }

        weaponsInPossession = weapons;
        equippedWeapons.Clear();
        equippedWeapons = alreadyEquippedWeapons;
    }

    private void OnConfirmEquipButtonClicked() {
        UpdateEquippedWeaponsList();
        confirmEquipSignal.Dispatch();
    }

    private void OnToggleValueChange(bool isChecked) {
        
        confirmEquipButton.interactable = true;

        if (isChecked) {
            numOfEquipped += 1;
            if (numOfEquipped == EQUIP_NUM_MAX) {
                UpdateInteractableOfUnchecked(false);
            }
        } else {
            if (numOfEquipped == EQUIP_NUM_MAX) {
                UpdateInteractableOfUnchecked(true);
            }
            numOfEquipped -= 1;
        }
    }

    private void UpdateInteractableOfUnchecked(bool interactable) {
        foreach(Toggle t in selections) {
            if (!t.isOn) {
                t.interactable = interactable;
            }
        }
    }

    private void UpdateEquippedWeaponsList() {
        equippedWeapons.Clear();
        for (int i = 0; i < selections.Count; ++i) {
            if (selections[i].isOn) {
                equippedWeapons.Add(weaponsInPossession[i]);
            }
        }        
    }

    private void OnStatusTabChanged(EStatusTab statusTab, bool isOn) {
        if (statusTab != THIS_STATUS_TAB) {
            return;
        }

        // De/Activate this view
        gameObject.SetActive(isOn);
    }

    // GameFlowStateChangeSignal will be fired when switching between map and status
    // usually this is when status panel first gets called, so everything will be
    // reset. Default EquipmentView to be hidden
    private void OnGameFlowStateChange(EGameFlowState gameFlowState) {
        if (gameFlowState != EGameFlowState.STATUS) { return; }

        OnStatusTabChanged(THIS_STATUS_TAB, false);
    }
}
