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

    public static ComboListFetcher GetInstance() {
        if (instance == null) {
            instance = new ComboListFetcher();
        }

        return instance;
    }

    public Dictionary<int, OneCombo> GetMap() {

        Dictionary<int, OneCombo> mapCopy = new Dictionary<int, OneCombo>();
        foreach(KeyValuePair<int, OneCombo> combo in comboMap) {
            mapCopy.Add(combo.Key, combo.Value);
        }

        return mapCopy;
    }

    public OneCombo GetComboById(int id) {
        return comboMap[id];
    }

    public string GetComboNameById(int id) {
        OneCombo aCombo;
        if (comboMap.TryGetValue(id, out aCombo)) {
            return aCombo.Name;
        } else {
            return "";
        }
    }

    public string GetComboChineseNameById(int id) {
        OneCombo aCombo;
        if (comboMap.TryGetValue(id, out aCombo)) {
            return aCombo.ChineseName;
        } else {
            return "";
        }
    }

    public int[] GetComboSkillIdsById(int id) {
        OneCombo aCombo;
        if (comboMap.TryGetValue(id, out aCombo)) {
            return aCombo.SkillIds;
        } else {
            return null;
        }
    }

    public ActionParams GetComboSkillParamsById(int id) {
        OneCombo aCombo;
        if (comboMap.TryGetValue(id, out aCombo)) {
            return aCombo.Arguments;           
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

        for(int i = 0; i < combosArray.Count; ++i) {

            if (combosArray[i] == null) {
                break;
            }

            OneCombo aCombo = ScriptableObject.CreateInstance(typeof(OneCombo)) as OneCombo;
            JsonUtility.FromJsonOverwrite(combosArray[i].ToString(), aCombo);
            aCombo.SerializeValidState(combosArray[i]["validState"]);
            aCombo.SerializeSkillReqAndArg(combosArray[i]["skillReqAndArg"] as JSONClass);
            comboMap.Add(aCombo.ComboId, aCombo);
        }
    }

}
