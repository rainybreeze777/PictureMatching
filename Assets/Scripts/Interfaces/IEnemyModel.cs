using System;
using System.Collections.Generic;

public interface IEnemyModel {

	void GenerateSequence();

	List<int> GetPrevGeneratedSequence();

	int GetPrevSequenceMask(); // Returns a binary hash code used to indicate which tile should appear unknown

}
