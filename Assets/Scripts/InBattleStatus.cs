using UnityEngine;
using System.Collections;

public class InBattleStatus {

	private int currentHealth = 100;
	private int maxHealth = 100;
	private int damage = 10;

	private bool isDead = false;

	public int CurrentHealth { get { return currentHealth; } }
	public int MaxHealth { get { return maxHealth; } }
	public int Damage { get { return damage; } }
	public bool IsDead { get { return isDead; } }

	public void DealDmg(int dmg) {
		currentHealth -= dmg;
		if (currentHealth <= 0) {
			isDead = true;
		}
	}

	public void ResetHealth() {
		currentHealth = maxHealth;
		isDead = false;
	}

}
