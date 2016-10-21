using System;
using UnityEngine;
using System.Collections.Generic;

public abstract class InBattleStatus : IInBattleStatus {

    public int CurrentHealth { get { return currentHealth; } }
    public int MaxHealth { get { return maxHealth; } }
    public int Damage { get { return damage; } }
    public int ComboDamage { get { return comboDamage; } }
    public bool IsDead { get { return isDead; } }

    [Inject]
    public ResetBattleSignal resetBattleSignal { get; set; }
    [Inject]
    public OneExchangeDoneSignal oneExchangeDoneSignal { get; set; }

    private int currentHealth = 100;
    private int maxHealth = 100;
    private int damage = 5;
    private int comboDamage = 20;
    private int skillEffectCountdown = 0;

    private bool isDead = false;
    private double takeDmgModifier = 1.0;

    private Dictionary<int, OneCombo> equippedComboMap = new Dictionary<int, OneCombo>();

    [PostConstruct]
    public void PostConstruct() {
        resetBattleSignal.AddListener(ResetHealth);
        oneExchangeDoneSignal.AddListener(OnOneExchangeDone);
        BindSignals();
    }

    public void ReceiveDmg(int dmg) {
        currentHealth -= (int) (dmg * takeDmgModifier);
        if (currentHealth <= 0) {
            isDead = true;
        }
        FireHealthUpdatedSignal();
    }
    
    public void ResetHealth() {
        currentHealth = maxHealth;
        isDead = false;
        FireHealthUpdatedSignal();
    }

    public void AddToHealth(int addAmount) {
        currentHealth += addAmount;
        if (currentHealth >= maxHealth) {
            currentHealth = maxHealth;
        }
        FireHealthUpdatedSignal();
    }

    public void UpdateReceiveDamageModifier(double modifier) {
        takeDmgModifier = modifier;
        skillEffectCountdown = 3; // TODO: Hardcode to 3 for now;
    }

    public Dictionary<int, OneCombo> GetEquippedCombos() {
        return equippedComboMap;
    }

    public void OnOneExchangeDone(Enum winner, int winningTile) {
        if (skillEffectCountdown > 0) {
            skillEffectCountdown--;
        }
        if (skillEffectCountdown == 0) {
            takeDmgModifier = 1.0;
        }
    }

    public void UpdateEquipWeapon(List<Weapon> equippedWeaponList) {
        equippedComboMap.Clear();
        ComboListFetcher fetcher = ComboListFetcher.GetInstance();
        foreach(Weapon w in equippedWeaponList) {
            List<int> comboIdList = w.GetComboIdList();
            foreach(int id in comboIdList) {
                equippedComboMap[id] = fetcher.GetComboById(id);
            }
        }
    }

    abstract protected void FireHealthUpdatedSignal();

    abstract protected void BindSignals();
}
