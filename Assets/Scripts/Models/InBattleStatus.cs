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

    [PostConstruct]
    public void BindSignals() {
        resetBattleSignal.AddListener(ResetHealth);
        oneExchangeDoneSignal.AddListener(OnOneExchangeDone);
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

    public void OnOneExchangeDone(Enum winner, int winningTile) {
        if (skillEffectCountdown > 0) {
            skillEffectCountdown--;
        }
        if (skillEffectCountdown == 0) {
            takeDmgModifier = 1.0;
        }
    }

    abstract public Dictionary<int, OneCombo> GetEquippedCombos();

    abstract protected void FireHealthUpdatedSignal();
}
