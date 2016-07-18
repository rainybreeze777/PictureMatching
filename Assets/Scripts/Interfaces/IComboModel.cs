using System;
using System.Collections.Generic;
using strange.extensions.signal.impl;

public interface IComboModel {

    void AddToCancelSequence(int tileNumber);

    List<int> GetCancelSeq();

    int NumOfTilesOnComboSequence { get; set; }

    void MakeCombo();

    void ClearCancelSequence();

    Signal<int> CancelAddedSignal { get; }
}
