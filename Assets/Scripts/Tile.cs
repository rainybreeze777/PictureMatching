using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	private BoardController boardController;
	private bool isSelected = false;

	private int row, column;

	public int Row { 
		get { return row; }
	}
	public int Column { 
		get { return column; }
	}

	public void Initialize(int row, int column) {
		this.row = row;
		this.column = column;
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

		if(isSelected) {
			boardController.TileSelected(this);
		} else {
			boardController.TileDeselected(this);
		}
	}
}
