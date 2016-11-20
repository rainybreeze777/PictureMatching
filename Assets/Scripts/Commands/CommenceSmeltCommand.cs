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
    [Inject]
    public ISmeltery smeltery { get; set; }

    public override void Execute() {
        playerStatus.DeductEssence(spentEssence);
        Weapon forgedWeapon = smeltery.SmeltWeapon(spentEssence, 0);
        playerStatus.ObtainWeapon(forgedWeapon);
    }
}
