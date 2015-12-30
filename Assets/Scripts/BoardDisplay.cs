using UnityEngine;
using System.Collections;

public class BoardDisplay : MonoBehaviour {

	private Board board;

	private Transform boardHolder;
	private const float xOffset = 5.5f;
	private const float yOffset = 0.8f;

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
					//the tiles array is used to grab the Array of Tile Sprites in Unity, so it starts from 0
					//however, the currentTile here is the number represented in Board.
					//In Board, 0 represents an empty tile, and valid tile index starts from 1.
					//This system should be changed later, possibly with some tile look up system
					//in order to avoid confusion.
					GameObject toInstantiate = tiles[currentTile - 1];

					GameObject instance = 
						Instantiate(toInstantiate
									, new Vector3(xOffset + c*1.45F, yOffset + r*1.45F, 0f)
									, Quaternion.identity)
						as GameObject;

					instance.transform.localScale = new Vector3(0.5F, 0.5F, 0);
					instance.transform.SetParent(boardHolder);
					
					//Get the actual Tile Script class in order to call its functions.
					Tile tile = instance.GetComponent<Tile>();
					//Pass in the currentTile for now, just to be consistent with Board's system of indexing.
					tile.Initialize(r, c, currentTile);
				}
			}
		}
	}
}
