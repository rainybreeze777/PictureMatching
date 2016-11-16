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

    public Signal<List<int>> smeltButtonClickedSignal = new Signal<List<int>>();

    internal void Init() {
        smeltButton.GetComponent<Button>().onClick.AddListener(() => {
                OnSmelt();
            });
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
}
