using UnityEngine;
using System.Collections.Generic;

public class InBattlePlayerStatus : InBattleStatus {

    [Inject]
    public PlayerHealthUpdatedSignal receivedDmgSignal { get; set; }

    protected override void FireHealthUpdatedSignal() {
        receivedDmgSignal.Dispatch();
    }

    protected override void BindSignals() {
        // Empty for now
    }
}
