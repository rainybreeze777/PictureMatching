using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : ScriptableObject {

    [SerializeField] private List<int> possessComboIdList = new List<int>();
    [SerializeField] private string weaponName;
    [SerializeField] private string weaponDescription;
    [SerializeField] private int tier;
    [SerializeField] private int id;
    [SerializeField] private EElements weaponElem;

    public List<int> GetComboIdList() {
        return new List<int>(possessComboIdList);
    }

    public string GetWeaponName() { return weaponName; }
    public string GetWeaponDesc() { return weaponDescription; }
    public int Tier { get { return tier; } }
    public int ID { get { return id; } }
    public EElements Elem { get { return weaponElem; } }

    public override bool Equals(object other) {
        if (other == null) { return false; }
        if (ReferenceEquals(this, other)) { return true; }
        if (typeof(Weapon) != other.GetType()) { return false; }
        Weapon otherWeapon = (Weapon) other;
        return possessComboIdList.Equals(otherWeapon.possessComboIdList) 
                && weaponName.Equals(otherWeapon.weaponName);
    }

    public override int GetHashCode() {
        int hash = 13;
        hash = (hash * 7) + possessComboIdList.GetHashCode();
        hash = (hash * 7) + weaponName.GetHashCode();
        hash = (hash * 7) + weaponDescription.GetHashCode();

        return hash;
    }

}
