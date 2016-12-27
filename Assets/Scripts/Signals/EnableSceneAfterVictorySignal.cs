using System;
using strange.extensions.signal.impl;

// Signal fired to indicate that this battle will enable
// a scene for a player after victory
// Parameters:
// 1. int: the gameSceneId to enable
public class EnableSceneAfterVictorySignal : Signal<int>
{
}