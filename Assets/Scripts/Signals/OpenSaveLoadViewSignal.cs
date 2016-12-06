using System;
using strange.extensions.signal.impl;

// Signal fired to open Save/Load view
// Params:
// bool: true means to open Save view, false means to open Load view
public class OpenSaveLoadViewSignal : Signal<bool>
{
}