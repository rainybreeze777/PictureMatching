using UnityEngine;
using System;
using System.Collections.Generic;
using Yarn.Unity;

[System.Serializable]
public class ProgressData {
    [SerializeField]
    private List<string> keys = new List<string>();
    [SerializeField]
    private List<string> values = new List<string>();
    [SerializeField]
    private List<Yarn.Value.Type> types = new List<Yarn.Value.Type>();

    public ProgressData(Dictionary<string, Yarn.Value> dict) {
        SerializeData(dict);
    }

    public void SerializeData(Dictionary<string, Yarn.Value> dict) {
        Clear();
        if (dict == null) { return; }
        foreach(var pair in dict) {
            keys.Add(pair.Key);
            values.Add(pair.Value.AsString);
            types.Add(pair.Value.type);
        }
    }

    public Dictionary<string, Yarn.Value> DeserializeData() {
        Dictionary<string, Yarn.Value> dict = new Dictionary<string, Yarn.Value>();
        for (int i = 0; i < keys.Count; ++i) {
            object value;

            switch (types[i]) {
            case Yarn.Value.Type.Number:
                float f = 0.0f;
                float.TryParse(values[i], out f);
                value = f;
                break;
            case Yarn.Value.Type.String:
                value = values[i];
                break;
            case Yarn.Value.Type.Bool:
                bool b = false;
                bool.TryParse(values[i], out b);
                value = b;
                break;
            case Yarn.Value.Type.Variable:
                // We don't support assigning default variables from other variables
                // yet
                Debug.LogErrorFormat("Can't set variable {0} to {1}: You can't " +
                    "set a default variable to be another variable, because it " +
                    "may not have been initialised yet.", keys[i], values[i]);
                continue;
            case Yarn.Value.Type.Null:
                value = null;
                break;
            default:
                throw new System.ArgumentOutOfRangeException();
            }

            try {
                dict.Add(keys[i], new Yarn.Value(value));
            } catch (ArgumentException e) {
                Debug.LogErrorFormat("Attempting to add duplicate key {0} with value {1} in " +
                    "ProgresData deserialization! The save file may be corrupt!", keys[i], values[i]);
            }
        }

        return dict;
    }

    public void Clear() {
        keys.Clear();
        values.Clear();
        types.Clear();
    }
}
