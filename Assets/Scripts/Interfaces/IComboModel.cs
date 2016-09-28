using System;
using System.Collections.Generic;
using strange.extensions.signal.impl;

public interface IComboModel {

    void AddToCancelSequence(int tileNumber);

    List<int> GetCancelSeq();

    void ClearCancelSequence();

    Signal<int> CancelAddedSignal { get; }

    void DeductPendingComboElems();

    void UpdatePendingComboId(int comboId);
}
