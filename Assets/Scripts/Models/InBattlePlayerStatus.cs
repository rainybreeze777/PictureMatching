using UnityEngine;
using System.Collections.Generic;

public class InBattlePlayerStatus : InBattleStatus {

	[Inject]
	public PlayerHealthUpdatedSignal receivedDmgSignal { get; set; }

	protected override void FireHealthUpdatedSignal() {
		receivedDmgSignal.Dispatch();
	}

    public override Dictionary<int, OneCombo> GetEquippedCombos() {
        return ComboListFetcher.GetInstance().GetMap();
    }
}
