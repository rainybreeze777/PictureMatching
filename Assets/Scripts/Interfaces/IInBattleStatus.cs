using System;
using System.Collections.Generic;

public interface IInBattleStatus {

    int CurrentHealth { get; }
    int MaxHealth { get; }
    int Damage { get; }
    bool IsDead { get; }

    void ReceiveDmg(int dmg);
    void AddToHealth(int addAmount);
    void ResetHealth();

    void UpdateReceiveDamageModifier(double modifier, int inEffectExchangeCount);
    void UpdateEquipWeapon(List<Weapon> equippedWeaponList);
    Dictionary<int, OneCombo> GetEquippedCombos();

}
