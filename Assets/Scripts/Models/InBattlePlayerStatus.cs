using UnityEngine;
using System.Collections;

public class InBattlePlayerStatus : InBattleStatus {

	[Inject]
	public PlayerHealthUpdatedSignal receivedDmgSignal { get; set; }

	protected override void FireHealthUpdatedSignal() {
		receivedDmgSignal.Dispatch();
	}

}
