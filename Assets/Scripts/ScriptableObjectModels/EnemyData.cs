using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class EnemyData : ScriptableObject {

    [SerializeField] private int enemyId;
    public int EnemyId { get { return enemyId; } }

    [SerializeField] private EElements preferredElem;
    public EElements PreferredElem { get { return preferredElem; } }

    [SerializeField] private int level;
    public int Level { get { return level; } }

    // Initial health
    [SerializeField] private int health;
    public int Health { get { return health; } }

    // Initial Damage
    [SerializeField] private int damage;
    public int Damage { get { return damage; } }

    public List<int> SkillIds { 
        get { 
            return new List<int>(skills.Keys);
        }
    }
    public Dictionary<int, SkillReqAndArg> AllSkillReqsAndArgs { 
        get {
            return new Dictionary<int, SkillReqAndArg>(skills); 
        }
    }
    public SkillReqAndArg GetSkillReqAndArgFromSkillId(int id) {
        return skills[id];
    }

    private Dictionary<int, SkillReqAndArg> skills = new Dictionary<int, SkillReqAndArg>();

    public void SerializeSkillReqAndArgs(JSONClass jsonClass) {

        foreach(KeyValuePair<string, JSONNode> node in jsonClass) {
            SkillReqAndArg skillReqAndArg = ScriptableObject.CreateInstance(typeof(SkillReqAndArg)) as SkillReqAndArg;
            JsonUtility.FromJsonOverwrite(node.Value.ToString(), skillReqAndArg);
            skillReqAndArg.InitRequirements();
            skillReqAndArg.SerializeArguments(node.Value["arguments"] as JSONArray); // JsonUtility does not support polymorphic array serialization
            skills.Add(Int32.Parse(node.Key), skillReqAndArg);
        }
    }
}
