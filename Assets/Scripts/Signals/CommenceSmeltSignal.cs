using System;
using System.Collections.Generic;
using strange.extensions.signal.impl;

// Signal fired when user clicks Smelt button in SmeltView
// Parameters:
// 1. List<int>: holds the essence spent on each element
//    - should be length of 5, order is Metal Wood Water Fire Earth
public class CommenceSmeltSignal : Signal<List<int>>
{
}