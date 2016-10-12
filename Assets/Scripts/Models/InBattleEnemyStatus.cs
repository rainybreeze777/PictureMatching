using UnityEngine;
using System.Collections.Generic;

public class InBattleEnemyStatus : InBattleStatus {

	[Inject]
	public EnemyHealthUpdatedSignal receivedDmgSignal { get; set; }

	protected override void FireHealthUpdatedSignal() {
		receivedDmgSignal.Dispatch();
	}

    public override Dictionary<int, OneCombo> GetEquippedCombos() {
        return null;
    }

}
