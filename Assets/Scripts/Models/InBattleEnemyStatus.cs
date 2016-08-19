using UnityEngine;
using System.Collections;

public class InBattleEnemyStatus : InBattleStatus {

	[Inject]
	public EnemyHealthUpdatedSignal receivedDmgSignal { get; set; }

	protected override void FireHealthUpdatedSignal() {
		receivedDmgSignal.Dispatch();
	}

}
