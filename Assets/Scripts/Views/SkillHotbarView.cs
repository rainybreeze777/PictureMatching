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

    [Inject]
    public UseSkillSignal useSkillSignal { get; set; }

    internal void Init() {
        // TODO: For now hard-code comboId with each button

        skill1Button.interactable = false;
        skill1Button.onClick.AddListener(() => {
                useSkillSignal.Dispatch(1);
            });
        skill2Button.interactable = false;
        skill2Button.onClick.AddListener(() => {
                useSkillSignal.Dispatch(2);
            });
        skill3Button.interactable = false;
        skill3Button.onClick.AddListener(() => {
                useSkillSignal.Dispatch(3);
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
        }
    }
}
