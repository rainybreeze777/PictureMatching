﻿using System;
using System.Collections.Generic;

public interface IPlayerStatus {

    int Health { get; }
    int Damage { get; }

    List<Weapon> GetPossessedWeapons();
    List<Weapon> GetEquippedWeapons();

    int MetalEssence { get; }
    int WoodEssence { get; }
    int WaterEssence { get; }
    int FireEssence { get; }
    int EarthEssence { get; }
    void DeductEssence(List<int> spentEssence);
}
