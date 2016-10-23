using UnityEngine;
using System.Collections.Generic;

public class InBattleEnemyStatus : InBattleStatus {

	[Inject]
	public EnemyHealthUpdatedSignal receivedDmgSignal { get; set; }

    protected override void FireHealthUpdatedSignal() {
        receivedDmgSignal.Dispatch();
    }

    protected override void FireEquipComboUpdatedSignal() {
    	// Empty for now
    }

    protected override void BindSignals() {
    	// Empty for now
    }

}
