using System;
using strange.extensions.signal.impl;

// Parameters: startRow, endRow, startColumn, endColumn
// These indices are used according to the model's board indexing system
// BoardView has a slightly different indexing system, which is
// most likely going to be 1 column and 1 row less than boardModel
public class TileRangeDestroyedSignal : Signal<int, int, int, int>
{
}