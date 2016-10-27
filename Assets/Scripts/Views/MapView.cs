using UnityEngine;
using UnityEngine.UI;
using System;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;

public class MapView : View {

    [SerializeField] private Button hqButton;
    [SerializeField] private Button smeltButton;
    [SerializeField] private Button arenaButton;

    public Signal<EMapChange> mapButtonClickedSignal = new Signal<EMapChange>();

    internal void Init() {
        hqButton.onClick.AddListener(() => {
            mapButtonClickedSignal.Dispatch(EMapChange.HQ);
        });
        smeltButton.onClick.AddListener(() => {
            mapButtonClickedSignal.Dispatch(EMapChange.SMELT);
        });
        arenaButton.onClick.AddListener(() => {
            mapButtonClickedSignal.Dispatch(EMapChange.ARENA);
        });
    }
}
