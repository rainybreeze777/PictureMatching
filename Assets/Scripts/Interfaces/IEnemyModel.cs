using System;
using System.Collections.Generic;

public interface IEnemyModel {

    void GenerateSequence();

    List<int> GetPrevGeneratedSequence();

    uint GetPrevSequenceMask(); // Returns a binary hash code used to indicate which tile should appear unknown

    void SetUpEnemyData(EnemyData enemy);

    EnemyData GetEnemyData();

    Dictionary<EElements, int> GetGatheredElems();

    void DeductSkillReqElems(int skillId);

}
