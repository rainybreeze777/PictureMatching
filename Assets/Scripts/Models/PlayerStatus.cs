using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerStatus : IPlayerStatus {

    public int Health { get { return health; } }
    public int Damage { get { return damage; }}
    public int MetalEssence { get { return essence[0]; } }
    public int WoodEssence { get { return essence[1]; } }
    public int WaterEssence { get { return essence[2]; } }
    public int FireEssence { get { return essence[3]; } }
    public int EarthEssence { get { return essence[4]; } }

    private int health = 500;
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
    [Inject]
    public PlayerEssenceGainedSignal playerEssenceGainedSignal { get; set; }

    public List<Weapon> GetPossessedWeapons() {
        List<Weapon> weaponsList = new List<Weapon>();

        foreach(Weapon w in weaponsInPossession) {
            if (!weaponsList.Contains(w)) {
                weaponsList.Add(w);
            }
        }

        return weaponsList;
    }

    public List<Weapon> GetEquippedWeapons() {
        return new List<Weapon>(equippedWeapons);
    }

    public PlayerStatus() {

    }

    [PostConstruct]
    public void PostConstruct() {

        equipWeaponUpdatedSignal.AddListener(UpdateEquippedWeapons);
        playerEssenceGainedSignal.AddListener(OnEssenceGained);
    }

    private void UpdateEquippedWeapons(List<Weapon> equippedWeapons) {
        this.equippedWeapons = equippedWeapons;
    }

    private void OnEssenceGained(List<int> gainedEssence) {
        Assert.IsTrue(gainedEssence.Count == 5);
        for (int i = 0; i < gainedEssence.Count; ++i) {
            essence[i] += gainedEssence[i];
        }

        playerInfoUpdatedSignal.Dispatch();
    }

    public bool DeductEssence(List<int> spentEssence) {
        Assert.IsTrue(spentEssence.Count == 5);

        for (int i = 0; i < spentEssence.Count; ++i) {
            if (essence[i] < spentEssence[i]) { 
                // Insufficient Essence
                weaponsInfoUpdatedSignal.Dispatch(EWeaponPossessionStatus.SMELT_INSUFFICIENT_ESSENCE, null);

                return false; 
            }
        }

        for (int i = 0; i < spentEssence.Count; ++i) {
            essence[i] -= spentEssence[i];
        }

        playerInfoUpdatedSignal.Dispatch();
        return true;
    }

    public void ObtainWeapon(Weapon w) {
        weaponsInPossession.Add(w);

        weaponsInfoUpdatedSignal.Dispatch(EWeaponPossessionStatus.ADD, w);
    }
}
