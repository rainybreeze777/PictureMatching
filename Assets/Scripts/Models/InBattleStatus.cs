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

    private int currentHealth = 100;
    private int maxHealth = 100;
    private int damage = 5;
    private int comboDamage = 20;

    private bool isDead = false;

    [PostConstruct]
    public void BindSignals() {
        resetBattleSignal.AddListener(ResetHealth);
    }

    public void ReceiveDmg(int dmg) {
        currentHealth -= dmg;
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

    abstract public Dictionary<int, OneCombo> GetEquippedCombos();

    abstract protected void FireHealthUpdatedSignal();
}
