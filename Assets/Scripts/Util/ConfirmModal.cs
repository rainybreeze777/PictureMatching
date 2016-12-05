using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ConfirmModal : MonoBehaviour {

    [SerializeField] private Text modalText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private Action affirmativeAction = ()=>{};
    private Action negativeAction = ()=>{};

    void Start() {
        gameObject.SetActive(false);
        yesButton.onClick.AddListener(OnYesButtonClicked);
        noButton.onClick.AddListener(OnNoButtonClicked);
    }

    public void ShowConfirmModal(string confirmMsg, Action affirmativeCallback) {
        ShowConfirmModal(confirmMsg, affirmativeCallback, ()=>{});
    }

    public void ShowConfirmModal(string confirmMsg, Action affirmativeCallback, Action negativeCallback) {
        modalText.text = confirmMsg;
        affirmativeAction = affirmativeCallback;
        negativeAction = negativeCallback;
        gameObject.SetActive(true);
    }

    private void OnYesButtonClicked() {
        gameObject.SetActive(false);
        affirmativeAction();
        affirmativeAction = ()=>{};
    }

    private void OnNoButtonClicked() {
        gameObject.SetActive(false);
        negativeAction();
        negativeAction = ()=>{};
    }
}
