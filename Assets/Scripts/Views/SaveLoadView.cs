using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;

public class SaveLoadView : View {

    [SerializeField] private GameObject saveLoadSlotTemplate;
    [SerializeField] private GameObject slotInfoPanel;
    [SerializeField] private GameObject content;
    [SerializeField] private Text titleText;
    [SerializeField] private Button backButton;

    [SerializeField] private ConfirmModal confirmModal;

    [Inject]
    public GameSaveFileOpSignal gameSaveFileOpSignal { get; set; }

    public Signal backButtonClickedSignal = new Signal();

    private bool isSaveView = true;

    private List<SlotInfo> slotsInfoArray;

    internal void Init() {
        backButton.onClick.AddListener(backButtonClickedSignal.Dispatch);
    }

    void Update() {
        if (!confirmModal.gameObject.activeInHierarchy && Input.GetKeyDown("escape")) {
            backButtonClickedSignal.Dispatch();
        }
    }

    public void OpenView(bool isSaveView, int slotsCount, List<GameSave> existingSaves) {
        this.isSaveView = isSaveView;
        gameObject.SetActive(true);
        if (isSaveView) {
            titleText.text = "保存游戏";
        } else {
            titleText.text = "读取游戏";
        }

        slotsInfoArray = new List<SlotInfo>();
        for (int i = 0; i < slotsCount; ++i) {
            int slotIndex = i; // This step cannot be omitted, otherwise the click listener will not behave correctly
            GameObject slot = Instantiate(saveLoadSlotTemplate) as GameObject;
            slot.transform.SetParent(content.transform, false);
            slot.GetComponent<ClickDetector>().clickSignal.AddListener(
                ()=>{ 
                    OnSlotClicked(slotIndex);
                }
            );
            slotsInfoArray.Add(new SlotInfo(slotIndex, slot));
        }
        foreach(GameSave gs in existingSaves) {
            slotsInfoArray[gs.SlotIndex].SaveGameInThisSlot(slotInfoPanel, gs.GetSaveTime());
        }
    }

    public void CloseView() {
        slotsInfoArray.Clear();
        slotsInfoArray = null;
        List<GameObject> children = new List<GameObject>();
        foreach(Transform child in content.transform) {
            children.Add(child.gameObject);
        }
        foreach(GameObject child in children) {
            Destroy(child);
        }
        gameObject.SetActive(false);
    }

    private void OnSlotClicked(int slotIndex) {

        Debug.Log("SlotIndex clicked: " + slotIndex);
        SlotInfo info = slotsInfoArray[slotIndex];

        if (isSaveView) {
            if (info.IsOccupied) {
                confirmModal.ShowConfirmModal(
                    "你确定要覆盖这个存档吗？"
                    , ()=>{
                        info.SaveGameInThisSlot(slotInfoPanel);
                        gameSaveFileOpSignal.Dispatch(info.SlotIndex, EGameSaveFileOp.SAVE);
                    }
                );
            } else {
                info.SaveGameInThisSlot(slotInfoPanel);
                gameSaveFileOpSignal.Dispatch(info.SlotIndex, EGameSaveFileOp.SAVE);
            }
        } else {
            gameSaveFileOpSignal.Dispatch(info.SlotIndex, EGameSaveFileOp.LOAD);
            backButtonClickedSignal.Dispatch();
        }
    }

    private class SlotInfo {
        private int m_slotIndex;
        public int SlotIndex {
            get { return m_slotIndex; }
            set { m_slotIndex = value; }
        }
        private bool m_isOccupied = false;
        public bool IsOccupied {
            get { return m_isOccupied; }
        }
        private GameObject m_slotGameObj;
        public GameObject SlotGameObject {
            get { return m_slotGameObj; }
        }
        private GameObject m_slotInfoObj;

        public SlotInfo(int index, GameObject slotGameObjectInstance) {
            m_slotIndex = index;
            m_slotGameObj = slotGameObjectInstance;
            m_isOccupied = false;
        }

        public void SaveGameInThisSlot(GameObject slotInfoPanelTemplate, string saveTime = null) {
            m_isOccupied = true;
            if (m_slotInfoObj == null) {
                m_slotInfoObj = Instantiate(slotInfoPanelTemplate) as GameObject;
                m_slotInfoObj.transform.SetParent(m_slotGameObj.transform, false);
            }

            if (saveTime != null && saveTime != "") {
                m_slotInfoObj.GetComponentInChildren<Text>().text = saveTime;
            } else {
                m_slotInfoObj.GetComponentInChildren<Text>().text = DateTime.Now.ToString(GameSave.GetDateFormat());
            }
        }
    }
}
