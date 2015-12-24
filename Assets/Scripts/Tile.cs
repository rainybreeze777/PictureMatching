using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	private BoardController boardController;
	private bool isSelected = false;

	private int row, column;
	private int tileNumber;

	public int Row { 
		get { return row; }
	}
	public int Column { 
		get { return column; }
	}
	public int TileNumber {
		get { return tileNumber; }
	}

	public void Initialize(int row, int column, int tileNumber) {
		this.row = row;
		this.column = column;
		this.tileNumber = tileNumber;
	}

	public GameObject GetGameObject() {
		return gameObject;
	}

	public void Deselect() {
		isSelected = false;
	}

	public bool Equals(Tile otherTile) {
		if ((object)otherTile == null)
			return false;

		return (row == otherTile.Row && column == otherTile.Column);
	}

	// Use this for initialization
	void Awake () {
		boardController = GameObject.Find("BoardController").GetComponent<BoardController>() as BoardController;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnMouseDown() {
		isSelected = !isSelected;

		Sprite loadSprite;
		string tmp = UtilFunctions.getSpriteInfo(0);
		string tmp2 = UtilFunctions.getSpriteInfo(1);

		if(isSelected) {
			boardController.TileSelected(this);
		} else {
			boardController.TileDeselected(this);
		}
	}
}
