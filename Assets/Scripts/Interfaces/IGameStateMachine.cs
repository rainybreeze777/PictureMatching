using System;
using System.Collections.Generic;
using strange.extensions.signal.impl;

public interface IGameStateMachine {
    EGameFlowState CurrentState { get; }
}
