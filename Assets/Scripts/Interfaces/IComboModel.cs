using System;
using System.Collections.Generic;
using strange.extensions.signal.impl;

public interface IComboModel {

    void AddToCancelSequence(int tileNumber);

    List<int> GetCancelSeq();

    void ResetBattleStatus();

    Signal<int> CancelAddedSignal { get; }

    void DeductComboElems(int comboId);

    void RefreshEquippedCombo();
}
