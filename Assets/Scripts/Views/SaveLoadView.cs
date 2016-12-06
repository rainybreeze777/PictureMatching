using UnityEngine;
using UnityEngine.UI;
using System;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;

public class SaveLoadView : View {

    [SerializeField] private GameObject saveLoadSlotTemplate;
    [SerializeField] private GameObject content;
    [SerializeField] private Text titleText;
    [SerializeField] private Button backButton;

    public Signal backButtonClickedSignal = new Signal();

    private bool isSaveView = true;

    internal void Init() {
        backButton.onClick.AddListener(backButtonClickedSignal.Dispatch);
    }

    public void OpenView(bool isSaveView) {
        this.isSaveView = isSaveView;
        gameObject.SetActive(true);
        if (isSaveView) {
            titleText.text = "保存游戏";
        } else {
            titleText.text = "读取游戏";
        }
    }
}
