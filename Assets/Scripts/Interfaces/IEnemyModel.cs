using System;
using System.Collections.Generic;

public interface IEnemyModel {

	void GenerateSequence();

	List<int> GetPrevGeneratedSequence();

}
