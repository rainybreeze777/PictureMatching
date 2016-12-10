using UnityEngine;
using System.Collections.Generic;

public class InBattleEnemyStatus : InBattleStatus, IInBattleEnemyStatus {

	[Inject]
	public EnemyHealthUpdatedSignal receivedDmgSignal { get; set; }

    private List<int> rewardEssence;

    protected override void FireHealthUpdatedSignal() {
        receivedDmgSignal.Dispatch();
    }

    protected override void FireEquipComboUpdatedSignal() {
    	// Empty for now
    }

    protected override void BindSignals() {
    	// Empty for now
    }

    public void InitWithEnemyData(EnemyData enemyData) {
        currentHealth = maxHealth = enemyData.Health;
        damage = enemyData.Damage;
        rewardEssence = enemyData.RewardEssence;
    }

    public List<int> GetRewardEssence() {
        return rewardEssence;
    }

}
