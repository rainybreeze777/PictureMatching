using System;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class SkillReqAndArg : ScriptableObject {

    [SerializeField] private int metal;
    [SerializeField] private int wood;
    [SerializeField] private int water;
    [SerializeField] private int fire;
    [SerializeField] private int earth;

    public int Metal() { return GetReqFromEElements(EElements.METAL); }
    public int Wood() { return GetReqFromEElements(EElements.WOOD); }
    public int Water() { return GetReqFromEElements(EElements.WATER); }
    public int Fire() { return GetReqFromEElements(EElements.FIRE); }
    public int Earth() { return GetReqFromEElements(EElements.EARTH); }
    public int GetReqFromEElements(EElements elem) {
        Assert.IsTrue(initialized, "SkillReqAndArg's elem requirements are not initialized! Make sure InitRequirements() is called after calling FromJsonOverwrite()!");
        return elemReq[elem];
    }

    private List<object> arguments;
    public ActionParams Arguments {
        get {
            ActionParams ap = new ActionParams();
            foreach (object arg in arguments) {
                ap.AddToParamList(arg);
            }
            return ap;
        }
    }
    public List<object> RawArguments { get { return arguments; } }


    private Dictionary<EElements, int> elemReq;

    private bool initialized = false;

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
                    Debug.LogError("SkillReqAndArg SerializeArguments Error: Unsupported serialization type " + jsonArray[i].Tag);
                    break;
            }
        }
    }

    public Dictionary<EElements, int> ElemRequirement() {
        return new Dictionary<EElements, int>(elemReq);
    }

    public void OnEnable() {
        arguments = new List<object>();
        elemReq = new Dictionary<EElements, int>();
    }

    public void InitRequirements() {
        elemReq.Add(EElements.METAL, metal);
        elemReq.Add(EElements.WOOD, wood);
        elemReq.Add(EElements.WATER, water);
        elemReq.Add(EElements.FIRE, fire);
        elemReq.Add(EElements.EARTH, earth);

        initialized = true;
    }
#if DEVELOPMENT_BUILD
    /* Development only methods. These methods are to be
        used with unit-testing classes exclusively! Calling
        these methods by anything other than unit-testing classes
        are strictly undefined and will not be able to compile
        when releasing a production build! */
    public void UnitTesting_SetElemReq(string elem, int amount) {
        if (elem.Equals("metal")) {
            metal = amount;
        } else if (elem.Equals("wood")) {
            wood = amount;
        } else if (elem.Equals("water")) {
            water = amount;
        } else if (elem.Equals("fire")) {
            fire = amount;
        } else if (elem.Equals("earth")) {
            earth = amount;
        }
    }

    public bool UnitTesting_IsInitialized() {
        return initialized;
    }
#endif
}
