using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyData : ScriptableObject {

    [SerializeField] private int enemyId;
    public int EnemyId { get { return enemyId; } }

    [SerializeField] private EElements preferredElem;
    public EElements PreferredElem { get { return preferredElem; } }

    [SerializeField] private int level;
    public int Level { get { return level; } }

    [SerializeField] private int health;
    public int Health { get { return health; } }

    [SerializeField] private int damage;
    public int Damage { get { return damage; } }

}
