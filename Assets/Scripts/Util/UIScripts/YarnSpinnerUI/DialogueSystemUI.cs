using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class DialogueSystemUI : Yarn.Unity.DialogueUIBehaviour {

    private Yarn.OptionChooser setSelectedOption;

    public delegate void LineObserver(int speakerId, string line);
    public delegate void InteractTargetObserver(List<string> targets);
    public delegate void OptionChosenObserver(int chosenOptionIndex);
    public delegate void ConvoCompleteObserver(string nextNode);
    public delegate void InitCombatObserver(int enemyId);
    public delegate void DialogueCompleteObserver();
    private List<LineObserver> lineObs = new List<LineObserver>();
    private List<InteractTargetObserver> intTarObs = new List<InteractTargetObserver>();
    private List<OptionChosenObserver> optObs = new List<OptionChosenObserver>();
    private List<ConvoCompleteObserver> comptObs = new List<ConvoCompleteObserver>();
    private List<InitCombatObserver> combatObs = new List<InitCombatObserver>();
    private List<DialogueCompleteObserver> dialComptOb = new List<DialogueCompleteObserver>();

    private bool readNextLine = false;
    private bool battleComplete = false;

    public void AddLineObserver(LineObserver ob) {
        lineObs.Add(ob);
    }

    public void AddInteractTargetObserver(InteractTargetObserver ob) {
        intTarObs.Add(ob);
    }

    public void AddOptionChosenObserver(OptionChosenObserver ob) {
        optObs.Add(ob);
    }

    public void AddConvoCompleteObserver(ConvoCompleteObserver ob) {
        comptObs.Add(ob);
    }

    public void AddInitCombatObserver(InitCombatObserver ob) {
        combatObs.Add(ob);
    }

    public void AddDialogueCompleteObserver(DialogueCompleteObserver ob) {
        dialComptOb.Add(ob);
    }

    public void ReadNextLine() {
        readNextLine = true;
    }

    public void SetBattleComplete() {
        battleComplete = true;
    }

    public void SelectOption(int opt) {
        if (setSelectedOption != null) {
            setSelectedOption(opt);
            setSelectedOption = null;
        } else {
            Debug.LogWarning("DialogueSystemUI received unexpected SelectOption call!");
        }

        foreach(OptionChosenObserver ob in optObs) {
            ob(opt);
        }
    }

    public override IEnumerator RunLine(Yarn.Line line) {

        string[] lineTokens = line.text.Split(new char[] {':'}, 2);

        foreach(LineObserver ob in lineObs) {
            ob(Int32.Parse(lineTokens[0]), lineTokens[1]);
        }

        while (!readNextLine) {
            yield return null;
        }

        readNextLine = false;
    }

    public override IEnumerator RunOptions(Yarn.Options optionsCollection,
                                           Yarn.OptionChooser optionChooser) {

        setSelectedOption = optionChooser;
        List<string> options = new List<string>();

        foreach (var optionString in optionsCollection.options) {
            options.Add(optionString);
        }
        foreach(InteractTargetObserver ob in intTarObs) {
            ob(options);
        }

        while (setSelectedOption != null) {
            yield return null;
        }
    }

    public override IEnumerator RunCommand(Yarn.Command command) {

        string[] cmdTokens = command.text.Split(' ');
        string cmdName = cmdTokens[0];

        List<string> parameters;

        if (cmdTokens.Length > 1) {
            parameters = new List<string>(cmdTokens);
            parameters.RemoveRange(0, 1);
        } else {
            parameters = new List<string>();
        }

        switch (cmdName) {
            case "init":
                if (parameters[0] == "battle") {
                    int enemyId = Int32.Parse(parameters[1]);
                    foreach(InitCombatObserver ob in combatObs) {
                        ob(enemyId);
                    }
                    while(!battleComplete) {
                        yield return null;
                    }
                    battleComplete = false;
                }
                break;
            default:
                Debug.LogWarning("DialogueSystemUI received unknown command " + cmdName + " from Yarn!");
                yield break;
        }
    }

    public override IEnumerator NodeComplete(string nextNode) {
        foreach(ConvoCompleteObserver ob in comptObs) {
            ob(nextNode);
        }
        yield break;
    }

    public override IEnumerator DialogueComplete() {
        foreach(DialogueCompleteObserver ob in dialComptOb) {
            ob();
        }
        yield break;
    }
}
