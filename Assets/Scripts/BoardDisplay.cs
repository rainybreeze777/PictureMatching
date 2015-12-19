using UnityEngine;
using System.Collections;

public class BoardDisplay : MonoBehaviour {

	private Board board;

	private Transform boardHolder;

	//0 = Metal
	//1 = Wood
	//2 = Water
	//3 = Fire
	//4 = Earth
	public GameObject[] tiles;

	// Use this for initialization
	void Awake () {
		board = Board.getInstance();

		board.GenerateBoard();

		BoardSetup();
	}

	// Update is called once per frame
	void Update () {
	
	}

	void BoardSetup () {
		boardHolder = new GameObject ("Board").transform;

		int numOfRows = board.numOfRows();
		int numOfColumns = board.numOfColumns();

		for (int r = 0; r < numOfRows; r++) {
			for (int c = 0; c < numOfColumns; c++) {
				int currentTile = board.GetTileAt(r,c);
				if (currentTile != -1 && currentTile != 0) {
					GameObject toInstantiate = tiles[currentTile - 1];

					GameObject instance = 
						Instantiate(toInstantiate
									, new Vector3(c*1.45F, r*1.45F, 0f)
									, Quaternion.identity)
						as GameObject;

						instance.transform.localScale = new Vector3(0.5F, 0.5F, 0);
						instance.transform.SetParent(boardHolder);
					
					Tile tile = instance.GetComponent<Tile>();
					tile.Initialize(r, c);
				}
			}
		}
	}
}
