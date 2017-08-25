using UnityEngine;
using System;
using System.Collections.Generic;
using Yarn.Unity;

public class ProgressData : IProgressData {

    [Inject]
    public AvailableScenesUpdateSignal availableScenesUpdateSignal { get; set; }

    private Dictionary<string, Yarn.Value> yarnVariables = new Dictionary<string, Yarn.Value>();
    private HashSet<string> visitedNodes = new HashSet<string>();
    private HashSet<ESceneChange> availableScenes = new HashSet<ESceneChange>();

    public void LoadFromGameSave(GameSave save) {

        List<string> keys = save.ProgressVarKeys;
        List<string> values = save.ProgressVarValues;
        List<Yarn.Value.Type> types = save.ProgressVarTypes;

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
                yarnVariables.Add(keys[i], new Yarn.Value(value));
            } catch (ArgumentException e) {
                Debug.LogErrorFormat("Attempting to add duplicate key {0} with value {1} in " +
                    "ProgresData deserialization! The save file may be corrupt!", keys[i], values[i]);
            }
        }

        visitedNodes = new HashSet<string>(save.VisitedNodes);
        availableScenes = new HashSet<ESceneChange>(save.AvailableGameScenes);
    }

    public List<string> GetProgressKeys() {
        return new List<string>(yarnVariables.Keys);
    }

    public List<string> GetProgressValues() {
        List<Yarn.Value> values = new List<Yarn.Value>(yarnVariables.Values);
        List<string> ret = new List<string>(values.Count);
        foreach (var val in values) {
            ret.Add(val.AsString);
        }

        return ret;
    }

    public List<Yarn.Value.Type> GetProgressValueTypes() {
        List<Yarn.Value> values = new List<Yarn.Value>(yarnVariables.Values);
        List<Yarn.Value.Type> ret = new List<Yarn.Value.Type>(values.Count);
        foreach (var val in values) {
            ret.Add(val.type);
        }
        return ret;
    }

    public List<string> GetVisitedNodes() {
        return new List<string>(visitedNodes);
    }

    public List<ESceneChange> GetAvailableScenes() {
        return new List<ESceneChange>(availableScenes);
    }

    public void SetValue(string variableName, object value) {
        Dict[variableName] = new Yarn.Value(value);
    }

    public Dictionary<string, Yarn.Value> Dict { get { return yarnVariables; } }

    public bool VisitedNode(string yarnNodeName) {
        bool alreadyVisited = visitedNodes.Contains(yarnNodeName);
        visitedNodes.Add(yarnNodeName);
        return alreadyVisited;
    }

    public void SetSceneAvailability(int sceneId, bool isAvailable) {
        if (Enum.IsDefined(typeof(ESceneChange), sceneId)) {
            ESceneChange scene = (ESceneChange) sceneId;
            if (isAvailable && !availableScenes.Contains(scene)) {
                availableScenes.Add(scene);
                availableScenesUpdateSignal.Dispatch(scene, EAvailScenesUpdateType.ADD);
            } else if (!isAvailable && availableScenes.Contains(scene)) {
                availableScenes.Remove(scene);
                availableScenesUpdateSignal.Dispatch(scene, EAvailScenesUpdateType.REMOVE);
            }
        } else {
            throw new ArgumentException("Unrecognized SceneId! The sceneId " + sceneId + " cannot be converted to ESceneChange!");
        }
    }
}
