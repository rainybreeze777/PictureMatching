﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : IPlayerStatus {

    public int Health { get { return health; } }
    public int Damage { get { return damage; }}
    public int MetalEssence { get { return essence[0]; } }
    public int WoodEssence { get { return essence[1]; } }
    public int WaterEssence { get { return essence[2]; } }
    public int FireEssence { get { return essence[3]; } }
    public int EarthEssence { get { return essence[4]; } }

    private int health = 60;
    private int damage = 10;

    private List<Weapon> weaponsInPossession = new List<Weapon>();
    private List<Weapon> equippedWeapons = new List<Weapon>();

    // Metal, Wood, Water, Fire, Earth
    private List<int> essence = new List<int>() { 0, 0, 0, 0, 0 };

    [Inject]
    public PlayerEquipWeaponUpdatedSignal equipWeaponUpdatedSignal { get; set; }
    [Inject]
    public PlayerWeaponsInfoUpdatedSignal weaponsInfoUpdatedSignal { get; set; }
    [Inject]
    public PlayerInfoUpdatedSignal playerInfoUpdatedSignal { get; set; }

    public List<Weapon> GetPossessedWeapons() {
        return new List<Weapon>(weaponsInPossession);
    }

    public List<Weapon> GetEquippedWeapons() {
        return new List<Weapon>(equippedWeapons);
    }

    public PlayerStatus() {
        object[] objs = Resources.LoadAll("Weapons", typeof(Weapon));
        // Can access all weapons for now
        for (int i = 0; i < objs.Length; ++i) {
            weaponsInPossession.Add((Weapon) objs[i]);
        }
    }

    [PostConstruct]
    public void PostConstruct() {
        equipWeaponUpdatedSignal.AddListener(UpdateEquippedWeapons);
    }

    private void UpdateEquippedWeapons(List<Weapon> equippedWeapons) {
        this.equippedWeapons = equippedWeapons;
    }
}