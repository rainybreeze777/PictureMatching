using System;
using System.Collections;

public interface IInBattleStatus {

	int CurrentHealth { get; }
	int MaxHealth { get; }
	int Damage { get; }
	int ComboDamage { get; }
	bool IsDead { get; }

	void ReceiveDmg(int dmg);
	void ResetHealth();

}
