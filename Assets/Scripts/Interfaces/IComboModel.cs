using System;
using System.Collections.Generic;

public interface IComboModel {

    void AddToCancelSequence(int tileNumber);

    List<int> GetCancelSeq();

    int NumOfTilesOnComboSequence { get; set; }

    void MakeCombo();

    void ClearCancelSequence();
}
