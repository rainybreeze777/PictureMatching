using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class EnemyCancellationGenerator {

	public static List<int> GenerateSequence() {
		List<int> seq = new List<int>();
		Random.seed = (int)System.DateTime.Now.Ticks;
		//Suppose Enemy right now can have from 9 to 15 cancellations
		int randomSequenceSize = (int) Random.Range(9F, 15.9999F);
		for (int i = 0; i < randomSequenceSize; i++) {
			Random.seed = (int)System.DateTime.Now.Ticks;
			seq.Add((int) Random.Range(1F, 5.9999F)); // 0 is reserved for empty
		}

		return seq;
	}

}
