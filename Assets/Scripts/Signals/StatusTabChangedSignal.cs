using System;
using strange.extensions.signal.impl;

// Fired when Status tab selection has changed
// Parameters:
// 1. EStatusTab: the tab that has changed
// 2. bool: SetActive of the tab
public class StatusTabChangedSignal : Signal<EStatusTab, bool>
{
}