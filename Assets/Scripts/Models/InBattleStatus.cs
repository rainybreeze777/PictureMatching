using UnityEngine;
using System.Collections;

public abstract class InBattleStatus : IInBattleStatus {

    private int currentHealth = 100;
    private int maxHealth = 100;
    private int damage = 10;

    private bool isDead = false;

    public int CurrentHealth { get { return currentHealth; } }
    public int MaxHealth { get { return maxHealth; } }
    public int Damage { get { return damage; } }
    public bool IsDead { get { return isDead; } }

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

    abstract protected void FireHealthUpdatedSignal();
}
