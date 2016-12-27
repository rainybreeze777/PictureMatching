using System;
using strange.extensions.signal.impl;

// Signal fired when the available scenes changes
// Parameters:
// 1. int: gameSceneId, as specified in Stage*Text.json
//			this value should be -1 if a batch_update is fired
// 2. EVailScenesUpdateType: the type of the update
public class AvailableScenesUpdateSignal : Signal<int, EAvailScenesUpdateType>
{
}