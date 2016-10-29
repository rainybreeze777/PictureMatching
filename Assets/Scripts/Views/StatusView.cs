using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

public class StatusView : View {

    [SerializeField] private Button swapToMapButton;

    public Signal swapToMapButtonClickedSignal = new Signal();

    internal void Init() {
        swapToMapButton.onClick.AddListener(() => {
            swapToMapButtonClickedSignal.Dispatch();
        });
    }
}
