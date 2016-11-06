using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;

public class PlayerInfoView : View {

    [SerializeField] private Text playerHealthText;
    [SerializeField] private Text playerDamageText;
    [SerializeField] private Text metalEssenceText;
    [SerializeField] private Text woodEssenceText;
    [SerializeField] private Text waterEssenceText;
    [SerializeField] private Text fireEssenceText;
    [SerializeField] private Text earthEssenceText;

    internal void Init() {
        
    }

    public void UpdateHealthText(int amount) {
        playerHealthText.text = "生命: " + amount;
    }
    public void UpdateDamageText(int amount) {
        playerDamageText.text = "伤害: " + amount;
    }
    public void UpdateMetalText(int amount) {
        metalEssenceText.text = "金灵气: " + amount;
    }
    public void UpdateWoodText(int amount) {
        woodEssenceText.text = "木灵气: " + amount;
    }
    public void UpdateWaterText(int amount) {
        waterEssenceText.text = "水灵气: " + amount;
    }
    public void UpdateFireText(int amount) {
        fireEssenceText.text = "火灵气: " + amount;
    }
    public void UpdateEarthText(int amount) {
        earthEssenceText.text = "土灵气: " + amount;
    }

}
