using System;
using SimpleJSON;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class OneCombo {

    [SerializeField]
    private string englishName;
    public string Name { get { return englishName; } }

    [SerializeField]
    private int id;
    public int ComboId { get { return id; } }

    [SerializeField]
    private int skillId;
    public int SkillId { get { return skillId; } }

    [SerializeField]
    private int metal;
    public int Metal { get { return metal; } }

    [SerializeField]
    private int wood;
    public int Wood { get { return wood; } }

    [SerializeField]
    private int water;
    public int Water { get { return water; } }

    [SerializeField]
    private int fire;
    public int Fire { get { return fire; } }

    [SerializeField]
    private int earth;
    public int Earth { get { return earth; } }

    private List<object> arguments;
    public List<object> Arguments { get { return arguments; } }

    public List<int> ElemRequirement() {
        List<int> req = new List<int>();

        req.Add(metal);
        req.Add(wood);
        req.Add(water);
        req.Add(fire);
        req.Add(earth);

        return req;
    }

    public void SerializeArguments(JSONArray jsonArray) {
        if (jsonArray == null)
            return; 

        if (arguments == null) {
            arguments = new List<object>();
        }

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

    public OneCombo(OneCombo other) {
        this.englishName = other.englishName;
        this.id = other.id;
        this.skillId = other.skillId;
        this.metal = other.metal;
        this.wood = other.wood;
        this.water = other.water;
        this.fire = other.fire;
        this.earth = other.earth;
        this.arguments = new List<object>(other.arguments);
    }
}
