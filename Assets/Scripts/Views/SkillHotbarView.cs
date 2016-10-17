using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

public class SkillHotbarView : View {

    [SerializeField] private Button skill1Button;
    [SerializeField] private Button skill2Button;
    [SerializeField] private Button skill3Button;
    [SerializeField] private Button skill4Button;
    [SerializeField] private Button skill5Button;

    [Inject]
    public UseComboSignal useComboSignal { get; set; }

    internal void Init() {
        // TODO: For now hard-code comboId with each button

        skill1Button.interactable = false;
        skill1Button.onClick.AddListener(() => {
                useComboSignal.Dispatch(1);
            });
        skill2Button.interactable = false;
        skill2Button.onClick.AddListener(() => {
                useComboSignal.Dispatch(2);
            });
        skill3Button.interactable = false;
        skill3Button.onClick.AddListener(() => {
                useComboSignal.Dispatch(3);
            });
        skill4Button.interactable = false;
        skill4Button.onClick.AddListener(() => {
                useComboSignal.Dispatch(4);
            });
        skill5Button.interactable = false;
        skill5Button.onClick.AddListener(() => {
                useComboSignal.Dispatch(5);
            });
    }

    public void SetComboPrepStatus(int comboId, bool isAvailable) {
        switch (comboId) {
            case 1:
                skill1Button.interactable = isAvailable;
                break;
            case 2:
                skill2Button.interactable = isAvailable;
                break;
            case 3:
                skill3Button.interactable = isAvailable;
                break;
            case 4:
                skill4Button.interactable = isAvailable;
                break;
            case 5:
                skill5Button.interactable = isAvailable;
                break;
        }
    }
}
