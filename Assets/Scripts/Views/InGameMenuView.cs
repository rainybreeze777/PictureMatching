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

        // This is necessary, because it prepares the confirmModal
        // on load, otherwise bugs may appear with confirmModal appearing
        confirmModal.gameObject.SetActive(true);
        confirmModal.gameObject.SetActive(false);

        saveButton.onClick.AddListener(()=> {
            openSaveLoadViewSignal.Dispatch(true);
        });
        loadButton.onClick.AddListener(()=>{
            openSaveLoadViewSignal.Dispatch(false);    
        });
        quitButton.onClick.AddListener(()=>{
            confirmModal.ShowConfirmModal("确定退出游戏？未保存的游戏进度将会遗失！", QuitGame);
        });
        backButton.onClick.AddListener(()=>{ backButtonClickedSignal.Dispatch(); });
    }

    public void OnEscKeyPressed() {
        if (!confirmModal.gameObject.activeInHierarchy) {
            gameObject.SetActive(!gameObject.activeInHierarchy);
        }
    }

    private void QuitGame() {
        Debug.LogWarning("Quit Application!");
        Application.Quit();
    }
}
