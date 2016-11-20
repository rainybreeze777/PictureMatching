using System;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using Random = UnityEngine.Random;

public class Smeltery : ISmeltery {

    // List of karma to weapon tier conversion threshold values
    // e.g.
    // 0 of karma points -> tier 1 weapons
    // 100 of each essence -> tier 1 + 2 weapons
    // 200 ... -> tier 1 + 2 + 3 weapons
    // ...
    private List<int> karmaTierConversion = new List<int>() {
        0, 100, 200, 300, 400, 500, 600
    };

    private WeaponsFetcher weaponsFetcher;

    [PostConstruct]
    public void PostConstruct() {
        weaponsFetcher = WeaponsFetcher.GetInstance();
    }

    public Weapon SmeltWeapon(List<int> spentEssence, int karma) {

        int avgKarma = 0;

        foreach(int essence in spentEssence) {
            int convert = EssenceToKarma(essence);
            Debug.Log("Convert essence " + essence + " to karma " + convert);
            avgKarma += convert;
        }

        avgKarma /= spentEssence.Count;
        Debug.Log("Average karma points: " + avgKarma);

        int resultKarma = karma + avgKarma;
        Debug.Log("Final karma points: " + resultKarma);

        int nextLargerIndex = karmaTierConversion.BinarySearch(resultKarma);
        if (nextLargerIndex < 0) {
            // Not an exact match, List.BinarySearch returns a negative number
            // that is the bitwise complement of the index of the next element
            // that is larger than given item.
            nextLargerIndex = ~nextLargerIndex - 1;
        }
        Debug.Log("Final highest weapon tier: " + (nextLargerIndex + 1));

        int randomTier = Random.Range(1, nextLargerIndex + 2); // nextLargerIndex + 1 is the highest tier, +1 to make it inclusive;

        List<Weapon> randomTierWeapons = weaponsFetcher.GetWeaponsByTier(randomTier);
        if (randomTierWeapons == null) {
            // Test code, for now assume that null means we achieved a tier thats not implemented
            randomTierWeapons = weaponsFetcher.GetWeaponsByTier(weaponsFetcher.GetHighestWeaponTier());
        }

        return randomTierWeapons[Random.Range(0, randomTierWeapons.Count)];
    }

    // Arbitrarily deduced formula from 2 points on a desired parabola
    private int EssenceToKarma(int essence) {
        if (essence < 1) { return 0; } // Minimum should be 1, reject anything thats less
        return (int) (300.0f * Mathf.Sqrt(2.0f / 499.0f) * Mathf.Sqrt(essence - 1));
    }

}
