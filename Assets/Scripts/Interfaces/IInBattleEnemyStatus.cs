using System;
using System.Collections.Generic;

public interface IInBattleEnemyStatus {

    void InitWithEnemyData(EnemyData enemyData);
    List<int> GetRewardEssence();

}
