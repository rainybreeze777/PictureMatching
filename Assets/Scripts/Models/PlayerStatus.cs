using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : IPlayerStatus {

    public int Health { get { return health; } }
    public int Damage { get { return damage; }}

    private int health = 60;
    private int damage = 10;

    private List<int> weaponsInPossession = new List<int>();
    private List<int> equippedWeapons = new List<int>();

    public List<int> GetPossessedWeaponIds() {
        return new List<int>(weaponsInPossession);
    }

    public List<int> GetEquippedWeaponIds() {
        return new List<int>(equippedWeapons);
    }
}
