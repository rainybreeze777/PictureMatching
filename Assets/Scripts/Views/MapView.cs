using UnityEngine;
using UnityEngine.UI;
using System;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;

public class MapView : View {

    [SerializeField] private Button arenaButton;
    [SerializeField] private Button swapToStatusButton;

    public Signal swapToStatusButtonClickedSignal = new Signal();
    public Signal<EMapChange> mapButtonClickedSignal = new Signal<EMapChange>();

    internal void Init() {
        arenaButton.onClick.AddListener(() => {
            mapButtonClickedSignal.Dispatch(EMapChange.METAL_ARENA);
        });
        swapToStatusButton.onClick.AddListener(() => {
            swapToStatusButtonClickedSignal.Dispatch();
        });
    }
}
