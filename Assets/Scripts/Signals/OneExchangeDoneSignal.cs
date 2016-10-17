using System;
using strange.extensions.signal.impl;

// Params:
// Enum: EOneExchangeWinner, indicates whether the player or enemy won this exchange
// int: the tile number that wins this round
public class OneExchangeDoneSignal : Signal<Enum, int>
{
}