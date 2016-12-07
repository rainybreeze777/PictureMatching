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

    public Signal backButtonClickedSignal = new Signal();

    private bool isSaveView = true;
    private const int SLOTS_COUNT = 20; 

    private List<SlotInfo> slotsInfoArray;

    internal void Init() {
        backButton.onClick.AddListener(backButtonClickedSignal.Dispatch);
    }

    void Update() {
        if (!confirmModal.gameObject.activeInHierarchy && Input.GetKeyDown("escape")) {
            backButtonClickedSignal.Dispatch();
        }
    }

    public void OpenView(bool isSaveView) {
        this.isSaveView = isSaveView;
        gameObject.SetActive(true);
        if (isSaveView) {
            titleText.text = "保存游戏";
        } else {
            titleText.text = "读取游戏";
        }

        slotsInfoArray = new List<SlotInfo>();
        for (int i = 0; i < SLOTS_COUNT; ++i) {
            int slotIndex = i; // This step cannot be omitted, otherwise the click listener will not behave correctly
            GameObject slot = Instantiate(saveLoadSlotTemplate) as GameObject;
            slot.transform.SetParent(content.transform, false);
            slot.GetComponent<ClickDetector>().clickSignal.AddListener(
                ()=>{ 
                    OnSlotClicked(slotIndex);
                }
            );
            slotsInfoArray.Add(new SlotInfo(slotIndex, slot, slotInfoPanel));
        }
    }

    private void OnSlotClicked(int slotIndex) {

        Debug.Log("SlotIndex clicked: " + slotIndex);
        SlotInfo info = slotsInfoArray[slotIndex];

        if (isSaveView) {
            if (info.IsOccupied) {
                confirmModal.ShowConfirmModal(
                    "你确定要覆盖这个存档吗？"
                    , ()=>{ info.SaveGameInThisSlot(slotInfoPanel); }
                );
            } else {
                info.SaveGameInThisSlot(slotInfoPanel);
            }
        } else {

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
        private string m_saveFilePath = "";
        public string SaveFilePath {
            get { return m_isOccupied ? m_saveFilePath : null; }
        }
        private GameObject m_slotGameObj;
        public GameObject SlotGameObject {
            get { return m_slotGameObj; }
        }
        private GameObject m_slotInfoObj;

        public SlotInfo(int index, GameObject slotGameObjectInstance, GameObject slotInfoPanelTemplate, string saveFilePath = null) {
            m_slotIndex = index;
            m_slotGameObj = slotGameObjectInstance;
            if (saveFilePath != null && saveFilePath != "") {
                m_isOccupied = true;
                m_saveFilePath = saveFilePath;
                m_slotInfoObj = Instantiate(slotInfoPanelTemplate) as GameObject;
                m_slotInfoObj.transform.SetParent(slotGameObjectInstance.transform, false);
            }
        }

        public void SaveGameInThisSlot(GameObject slotInfoPanelTemplate) {
            m_isOccupied = true;
            if (m_slotInfoObj != null) {

            } else {
                m_slotInfoObj = Instantiate(slotInfoPanelTemplate) as GameObject;
                m_slotInfoObj.transform.SetParent(m_slotGameObj.transform, false);
            }
        }
    }
}
