using System;
using System.Collections.Generic;
using Yarn.Unity;

public interface IProgressData {

    void LoadFromGameSave(GameSave save);
    List<string> GetProgressKeys();
    List<string> GetProgressValues();
    List<Yarn.Value.Type> GetProgressValueTypes();
    List<string> GetVisitedNodes();
    Dictionary<string, Yarn.Value> Dict { get; }
    bool VisitedNode(string yarnNodeName);
    void SetValue(string variableName, object value);
}
