using UnityEngine;
using UnityEngine.UI;
using System;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;

public class InGameMenuView : View {

    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button backButton;
    [SerializeField] private ConfirmModal confirmModal;

    [Inject]
    public OpenSaveLoadViewSignal openSaveLoadViewSignal { get; set; }

    public Signal backButtonClickedSignal = new Signal();

    internal void Init() {

        saveButton.onClick.AddListener(()=> {
            Debug.Log("saveButton clicked");
            openSaveLoadViewSignal.Dispatch(true);
        });
        loadButton.onClick.AddListener(()=>{
            Debug.Log("loadButton clicked");
            openSaveLoadViewSignal.Dispatch(false);    
        });
        quitButton.onClick.AddListener(()=>{
            confirmModal.ShowConfirmModal("确定退出游戏？未保存的游戏进度将会遗失！", QuitGame);
        });
        backButton.onClick.AddListener(()=>{ backButtonClickedSignal.Dispatch(); });
    }

    private void QuitGame() {
        Debug.LogWarning("Quit Application!");
        Application.Quit();
    }
}
