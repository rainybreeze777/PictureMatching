using System;
using UnityEngine;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;

public class ComboListFetcher {

    private JSONNode comboJsonSheet = null;
    private JSONArray combosArray = null;

    private static ComboListFetcher instance = null;

    private Dictionary<int, OneCombo> comboMap = new Dictionary<int, OneCombo>();
    // In-cache quick access
    private Dictionary<int, List<int>> comboList = new Dictionary<int, List<int>>();

    private TileInfoFetcher tileInfoFetcher;

    public static ComboListFetcher GetInstance() {
        if (instance == null) {
            instance = new ComboListFetcher();
        }

        return instance;
    }

    public Dictionary<int, List<int>> GetList() {
        return comboList;
    }

    public string GetComboNameById(int id) {
        OneCombo aCombo;
        if (comboMap.TryGetValue(id, out aCombo)) {
            return aCombo.Name;
        } else {
            return "";
        }
    }

    public int GetComboSkillIdById(int id) {
        OneCombo aCombo;
        if (comboMap.TryGetValue(id, out aCombo)) {
            return aCombo.SkillId;
        } else {
            return -1;
        }
    }

    public ActionParams GetComboSkillParamsById(int id) {
        OneCombo aCombo;
        if (comboMap.TryGetValue(id, out aCombo)) {

            ActionParams ap = new ActionParams();
            foreach (object arg in aCombo.Arguments) {
                ap.AddToParamList(arg);
            }
            return ap;
        } else {
            return null;
        }
    }

    private ComboListFetcher () {

        try {
            comboJsonSheet = JSON.Parse((Resources.Load("ComboList") as TextAsset).text);
        } catch (Exception ex) {
            Debug.LogError(ex.ToString());
        }
        combosArray = comboJsonSheet["combos"] as JSONArray;

        tileInfoFetcher = TileInfoFetcher.GetInstance();

        for(int i = 0; i < combosArray.Count; ++i) {

            if (combosArray[i] == null) {
                break;
            }

            OneCombo aCombo = JsonUtility.FromJson<OneCombo>(combosArray[i].ToString());
            aCombo.SerializeArguments(combosArray[i]["arguments"] as JSONArray); // JsonUtility does not support polymorphic array serialization
            comboMap.Add(aCombo.ComboId, aCombo);
            // List that tracks tiles using tile numbers instead of names
            List<int> tileNumberList = new List<int>();
            foreach(string tileName in aCombo.ComboSeq) {
                tileNumberList.Add(tileInfoFetcher.GetTileNumberFromName(tileName));
            }
            comboList.Add(aCombo.ComboId, tileNumberList);
        }
    }

}
