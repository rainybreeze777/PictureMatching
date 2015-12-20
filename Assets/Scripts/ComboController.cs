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

	private List<GameObject> onScreenSequence = new List<GameObject>();
	private int onScreenSequenceCount = 0;

	public const float movementSpeed = 10.0f;
	private const float distanceBetweenTile = 2.0f;
	private const int numOfTilesOnComboSequence = 5;

	private Transform comboDisplayer;

	void Awake() {
		comboDisplayer = new GameObject ("ComboDisplayer").transform;
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
						, new Vector3((onScreenSequenceCount - 1) * distanceBetweenTile, 0F, 0F)
						, Quaternion.identity) as GameObject;

		instance.transform.localScale = new Vector3(0.5F, 0.5F, 0);
		instance.transform.SetParent(comboDisplayer);

		onScreenSequence.Add(instance);


	}

}
