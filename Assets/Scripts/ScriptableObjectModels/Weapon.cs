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
                && weaponName.Equals(otherWeapon.weaponName)
                && tier == otherWeapon.tier
                && id == otherWeapon.id
                && weaponElem == otherWeapon.weaponElem;
    }

    public override int GetHashCode() {
        int hash = 13;
        hash = (hash * 7) + possessComboIdList.GetHashCode();
        hash = (hash * 7) + weaponName.GetHashCode();
        hash = (hash * 7) + weaponDescription.GetHashCode();

        return hash;
    }

#if DEVELOPMENT_BUILD
    /* Development only methods. These methods are to be
        used with unit-testing classes exclusively! Calling
        these methods by anything other than unit-testing classes
        are strictly undefined and will not be able to compile
        when releasing a production build! */
    public void UnitTesting_SetPossessComboIdList(List<int> comboIdList) {
        possessComboIdList = comboIdList;
    }

    public void UnitTesting_SetWeaponName(string name) {
        weaponName = name;
    }

    public void UnitTesting_SetWeaponDescription(string desc) {
        weaponDescription = desc;
    }
    public void UnitTesting_SetTier(int tier) {
        this.tier = tier;
    }
    public void UnitTesting_SetId(int id) {
        this.id = id;
    }
    public void UnitTesting_SetWeaponElem(EElements elem) {
        this.weaponElem = elem;
    }
#endif
}
