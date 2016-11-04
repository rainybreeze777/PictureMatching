using System;
using System.Collections.Generic;

public interface IPlayerStatus {

    int Health { get; }
    int Damage { get; }

    List<int> GetPossessedWeaponIds();
    List<int> GetEquippedWeaponIds();
}
