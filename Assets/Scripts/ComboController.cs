using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComboController : MonoBehaviour {

	//0 = Metal
	//1 = Wood
	//2 = Water
	//3 = Fire
	//4 = Earth
	public GameObject[] tiles;
	private List<int> cancelSequence = new List<int>();
	//List used to track the range of formed combos
	//For example, combo may start from index 2 and end at index 6
	//Then comboTracker will be [2, 6]
	private List<int> comboTracker = new List<int>();

	private List<GameObject> onScreenSequence = new List<GameObject>();
	private int onScreenSequenceCount = 0;

	private const float distanceBetweenTile = 2.0f;
	private const int numOfTilesOnComboSequence = 5;
	private const float xOffset = 6.5f;
	private const float yOffset = 0.77f;

	private Transform comboDisplayer;
	private ComboTree comboTree;
	private ComboListFetcher comboListFetcher;

	private GameObject comboButton;

	private int comboStart, comboEnd;

	void Awake() {
		comboButton = GameObject.Find("MakeComboButton");
		comboButton.SetActive(false);
		comboDisplayer = new GameObject ("ComboDisplayer").transform;
		comboTree = ComboTree.GetInstance();
		comboListFetcher = ComboListFetcher.GetInstance();
		foreach(List<int> combo in comboListFetcher.GetList()) {
			comboTree.AddCombo(combo, "nameGoesHere");
		}
	}

	public void ClearCancelSequence() {
		cancelSequence.Clear();
	}

	public void AddToCancelSequence(int tileNumber)
	{
		cancelSequence.Add(tileNumber);

		GameObject toInstantiate = tiles[tileNumber - 1];

		if (onScreenSequenceCount >= numOfTilesOnComboSequence) {
			GameObject headTile = onScreenSequence [0];
			onScreenSequence.RemoveAt (0);
			Destroy (headTile);

			foreach (GameObject aTile in onScreenSequence) {
				//For now there is no animations
				//Animation of movement needs some investigation
				aTile.transform.Translate (Vector3.left * distanceBetweenTile);
			}
		} else {
			onScreenSequenceCount++;
		}

		GameObject instance = 
			Instantiate(toInstantiate
						, new Vector3( xOffset + (onScreenSequenceCount - 1) * distanceBetweenTile, yOffset, 0F)
						, Quaternion.identity) as GameObject;

		instance.transform.localScale = new Vector3(0.5F, 0.5F, 0);
		instance.transform.SetParent(comboDisplayer);

		onScreenSequence.Add(instance);

		int startIndex = System.Math.Max(0, cancelSequence.Count - numOfTilesOnComboSequence);
		//Combo length is at least 2
		for (int i = startIndex; i < cancelSequence.Count - 2; i++ ) {

			List<int> subSequence = cancelSequence.GetRange(i, cancelSequence.Count - i);
			string comboName = comboTree.GetCombo(subSequence);
			if (!comboName.Equals("")) {
				comboButton.SetActive(true);
				comboStart = i;
				comboEnd = cancelSequence.Count - i;
				break;
			}
			comboButton.SetActive(false);
		}
	}

	public void MakeCombo() {
		comboTracker.Add(comboStart);
		comboTracker.Add(comboEnd);
	}

	public List<int> GetCancelSeq() {
		return cancelSequence;
	}

}
