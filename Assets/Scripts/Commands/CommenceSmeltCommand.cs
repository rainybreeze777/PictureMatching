using System;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.command.impl;

public class CommenceSmeltCommand : Command
{
    [Inject]
    public List<int> spentEssence { get; set; }
    [Inject]
    public IPlayerStatus playerStatus { get; set; }

    public override void Execute() {
        playerStatus.DeductEssence(spentEssence);
    }
}
