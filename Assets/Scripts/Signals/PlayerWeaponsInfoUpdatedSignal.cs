using System;
using System.Collections.Generic;
using strange.extensions.signal.impl;

// Signal used to inform that the weapons in player's possession have updated
// Parameters:
// 1. EWeaponPossessionStatus: enum that represents the info update type
// 2. Weapon: the weapon thats updated
public class PlayerWeaponsInfoUpdatedSignal : Signal<EWeaponPossessionStatus, Weapon>
{
}