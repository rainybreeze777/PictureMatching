using System;
using System.Collections.Generic;
using strange.extensions.signal.impl;

public interface IComboModel {

    void AddToCancelSequence(int tileNumber);

    List<int> GetCancelSeq();

    int MakeCombo();

    void ClearCancelSequence();

    Signal<int> CancelAddedSignal { get; }
}
