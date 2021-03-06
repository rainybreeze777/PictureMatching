using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Yarn.Unity;

public class ProgressVariableStorage : VariableStorageBehaviour
{

    [Inject]
    public IProgressData progressData { get; set; }

    [PostConstruct]
    public void PostConstruct() {
        // Register Functions that will be used by Yarn
        Yarn.Dialogue dialogue = (gameObject.GetComponent(typeof(DialogueRunner)) as DialogueRunner).dialogue;
        // visitedNode
        dialogue.library.RegisterFunction("visitedNode", 1, delegate(Yarn.Value[] parameters) {
            return progressData.VisitedNode(parameters[0].AsString);
        });
        // setSceneAvailability
        dialogue.library.RegisterFunction("setSceneAvailability", 2, delegate(Yarn.Value[] parameters) {
            progressData.SetSceneAvailability(Int32.Parse(parameters[0].AsString), parameters[1].AsBool);
        });

        // Make first scene available
        progressData.SetSceneAvailability(1, true);
    }

	// Not Implemented yet
	public override void ResetToDefaults () {

	}

    public override void SetValue (string variableName, Yarn.Value value)
    {
        progressData.Dict[variableName] = value;
    }

    public override Yarn.Value GetValue (string variableName)
    {
        return progressData.Dict[variableName];
    }

	// Erase all variables
	public override void Clear ()
	{
        progressData.Dict.Clear();
	}
}
