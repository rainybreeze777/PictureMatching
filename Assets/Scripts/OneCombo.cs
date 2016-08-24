using System;
using SimpleJSON;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class OneCombo {

    [SerializeField]
    private string englishName;
    public string Name
    {
        get { return englishName; }
    }

    [SerializeField]
    private int id;
    public int ComboId
    {
        get { return id; }
    }

    [SerializeField]
    private int skillId;
    public int SkillId
    {
        get { return skillId; }
    }

    [SerializeField]
    private List<string> comboSeq;
    public List<string> ComboSeq
    {
        get { return new List<string>(comboSeq); }
    }

    private List<object> arguments = new List<object>();
    public List<object> Arguments
    {
        get { return arguments; }
    }
    public void SerializeArguments(JSONArray jsonArray) {
        if (jsonArray == null)
            return; 

        for (int i = 0; i < jsonArray.Count; ++i) {
            switch(jsonArray[i].Tag) {
                case JSONBinaryTag.Value:
                    arguments.Add(jsonArray[i].Value);
                    break;
                case JSONBinaryTag.IntValue:
                    arguments.Add(jsonArray[i].AsInt);
                    break;
                case JSONBinaryTag.DoubleValue:
                    arguments.Add(jsonArray[i].AsDouble);
                    break;
                case JSONBinaryTag.BoolValue:
                    arguments.Add(jsonArray[i].AsBool);
                    break;
                // No Float, JSON doesn't support Float
                default:
                    Debug.LogError("OneCombo SerializeArguments Error: Unsupported serialization type " + jsonArray[i].Tag);
                    break;
            }
        }
    }
}
