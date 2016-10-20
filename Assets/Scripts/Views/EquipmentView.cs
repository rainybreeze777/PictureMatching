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

    public Signal confirmEquipSignal = new Signal();
    private List<Weapon> weapons = new List<Weapon>();
    private List<Toggle> selections = new List<Toggle>();

    internal void Init() {

        object[] objs = Resources.LoadAll("Weapons", typeof(Weapon));

        for (int i = 0; i < objs.Length; ++i) {
            weapons.Add((Weapon) objs[i]);
        }

        // foreach (Weapon w in weapons) {
        //     Toggle newToggle = new Toggle();
        //     newToggle.transform.SetParent(weaponEquipPanel);

            
            
        //     selections.Add(newToggle);
        // }

        confirmEquipButton.GetComponent<Button>().onClick.AddListener(() => {
            confirmEquipSignal.Dispatch();
        });
    }

    private void OnConfirmEquipButtonClicked() {
        confirmEquipSignal.Dispatch();
    }

}
