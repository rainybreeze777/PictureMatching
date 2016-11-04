using System;
using System.Collections.Generic;

public interface IPlayerStatus {

    int Health { get; }
    int Damage { get; }

    List<Weapon> GetPossessedWeapons();
    List<Weapon> GetEquippedWeapons();
}
