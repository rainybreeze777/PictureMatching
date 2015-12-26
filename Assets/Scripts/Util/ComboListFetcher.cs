using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComboListFetcher {

	private static ComboListFetcher instance = null;

	private List<List<int>> comboList;

	public static ComboListFetcher GetInstance() {
		if (instance == null) {
			instance = new ComboListFetcher();
		}

		return instance;
	}

	public List<List<int>> GetList() {
		return comboList;
	}

	private ComboListFetcher () {
		int[] combo1Array = { 2, 2, 2, 2 };
		int[] combo2Array = { 2, 2, 3, 4 };
		List<int> combo1 = new List<int>(combo1Array);
		List<int> combo2 = new List<int>(combo2Array);

		comboList = new List<List<int>>() { combo1, combo2 };
	}

}
