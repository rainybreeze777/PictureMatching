using System;
using strange.extensions.signal.impl;

// Signal fired when the available scenes changes
// Parameters:
// 1. ESceneChange: the scene enum, as specified in ESceneChange
// 2. EVailScenesUpdateType: the type of the update
public class AvailableScenesUpdateSignal : Signal<ESceneChange, EAvailScenesUpdateType>
{
}
