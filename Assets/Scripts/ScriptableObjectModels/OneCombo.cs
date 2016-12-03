using System;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class OneCombo : ScriptableObject {

    [SerializeField]
    private string englishName;
    public string Name { get { return englishName; } }

    [SerializeField]
    private string chineseName;
    public string ChineseName { get {  return chineseName; } }

    [SerializeField]
    private int id;
    public int ComboId { get { return id; } }

    [SerializeField]
    private int[] skillIds;
    public int[] SkillIds { 
        get { 
            int[] newSkillIds = new int[skillIds.Length];
            skillIds.CopyTo(newSkillIds, 0);
            return newSkillIds; 
        } 
    }

    public int Metal() { return GetReqFromEElements(EElements.METAL); }
    public int Wood() { return GetReqFromEElements(EElements.WOOD); } 
    public int Water() { return GetReqFromEElements(EElements.WATER); }
    public int Fire() { return GetReqFromEElements(EElements.FIRE); }
    public int Earth() { return GetReqFromEElements(EElements.EARTH); }
    public int GetReqFromEElements(EElements elem) {
        return skillReqAndArg.GetReqFromEElements(elem);
    }

    public List<object> RawArguments { get { return skillReqAndArg.RawArguments; } }
    public ActionParams Arguments { get { return skillReqAndArg.Arguments; } }

    public Dictionary<EElements, int> ElemRequirement() {
        return skillReqAndArg.ElemRequirement();
    }

    private SkillReqAndArg skillReqAndArg = null;

    public void SerializeSkillReqAndArg(JSONClass jsonClass) {
        skillReqAndArg = ScriptableObject.CreateInstance(typeof(SkillReqAndArg)) as SkillReqAndArg;
        JsonUtility.FromJsonOverwrite(jsonClass.ToString(), skillReqAndArg);
        skillReqAndArg.InitRequirements();
        skillReqAndArg.SerializeArguments(jsonClass["arguments"] as JSONArray); // JsonUtility does not support polymorphic array serialization
    }
}
