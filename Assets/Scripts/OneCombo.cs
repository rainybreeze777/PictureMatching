using System;
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
}
