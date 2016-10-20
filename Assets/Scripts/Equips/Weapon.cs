using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : ScriptableObject {

    [SerializeField] private List<int> possessComboIdList = new List<int>();
    [SerializeField] private string weaponName;
    [SerializeField] private string weaponDescription;

    public List<int> GetComboIdList() {
        return new List<int>(possessComboIdList);
    }

    public string GetWeaponName() { return weaponName; }
    public string GetWeaponDesc() { return weaponDescription; }

}
