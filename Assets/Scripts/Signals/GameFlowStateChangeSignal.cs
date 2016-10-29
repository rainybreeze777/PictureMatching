using System;
using strange.extensions.signal.impl;

// Signal to indicate a game flow state change, i.e. whether the player
// is currently in battle, on map, or viewing status
// Parameters:
// EGameFlowState: the state transitioning into
public class GameFlowStateChangeSignal : Signal<EGameFlowState>
{
}