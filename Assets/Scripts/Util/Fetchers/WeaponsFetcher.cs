using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WeaponsFetcher {

    private static WeaponsFetcher fetcher = null;

    // each element is 1 tier, index = weapon tier - 1
    private List<List<Weapon>> allTierWeapons;
    // map of id to Weapon
    private Dictionary<int, Weapon> idToWeaponMap;

    private WeaponsFetcher() {
        allTierWeapons = new List<List<Weapon>>();
        idToWeaponMap = new Dictionary<int, Weapon>();

        object[] objs = Resources.LoadAll("Weapons", typeof(Weapon));
        for (int i = 0; i < objs.Length; ++i) {

            Weapon aWeapon = (Weapon) objs[i];

            if (aWeapon.Tier > allTierWeapons.Count) {
                for (int tierCounter = allTierWeapons.Count; tierCounter < aWeapon.Tier; ++tierCounter) {
                    // Fill intermediate tiers with empty list first
                    // Count is equal to the currently processed highest
                    // tier number weapon
                    allTierWeapons.Add(new List<Weapon>());
                }
            }
            allTierWeapons[aWeapon.Tier - 1].Add(aWeapon);

            if (idToWeaponMap.ContainsKey(aWeapon.ID)) {
                string errMsg = "WeaponsFetcher Init Error: A weapon with the same ID already exists in the dictionary!\n";
                errMsg += "Duplicating ID: " + aWeapon.ID + "\n";
                errMsg += "Adding Weapon: " + aWeapon.GetWeaponName() + "\n";
                errMsg += "Existing Weapon: " + idToWeaponMap[aWeapon.ID].GetWeaponName() + "\n";

                Debug.LogError(errMsg);
                continue;
            }
            
            idToWeaponMap.Add(aWeapon.ID, aWeapon);
        }
    }

    public static WeaponsFetcher GetInstance() {
        if (fetcher == null)
            fetcher = new WeaponsFetcher();

        return fetcher;
    }

    public int GetHighestWeaponTier() {
        return allTierWeapons.Count;
    }

    public List<Weapon> GetWeaponsByTier(int tier) {
        if (tier > allTierWeapons.Count) {
            return null;
        }

        return allTierWeapons[tier - 1];
    }

    public Weapon GetWeaponByID(int id) {
        Weapon aWeapon;
        if (idToWeaponMap.TryGetValue(id, out aWeapon)) {
            return aWeapon;
        } else {
            return null;
        }
    }
}
