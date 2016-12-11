using System;
using System.Collections.Generic;
using strange.extensions.signal.impl;

// Signal to start a battle
// Parameters:
// 1. int: the enemyId
// 2. List<int>: the essence the player wish to inject for battle
public class EngageCombatSignal : Signal<int, List<int>>
{
}