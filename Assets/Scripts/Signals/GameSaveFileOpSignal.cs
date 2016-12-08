using System;
using strange.extensions.signal.impl;

// Signal fired when player clicks a save slot and performs an operation to the save file
// Parameters:
// 1. int: the slot index player selected
// 2. EGameSaveFileOp: the operation of this signal
public class GameSaveFileOpSignal : Signal<int, EGameSaveFileOp>
{
}