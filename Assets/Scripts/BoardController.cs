﻿using UnityEngine;
using System.Collections;

public class BoardController : MonoBehaviour {

	private Board board;

	private Tile tile1;
	private Tile tile2;

	public void TileSelected(Tile aTile) {
		if (tile1 == null) {
			tile1 = aTile;
		} else if (tile2 == null) {
			tile2 = aTile;
		}

		if (tile1 != null && tile2 != null) {
			if (board.isRemovable(tile1.Row, 
									tile1.Column, 
									tile2.Row, 
									tile2.Column)) {
				board.remove(tile1.Row, tile1.Column, tile2.Row, tile2.Column);
				Destroy(tile1.GetGameObject());

				Destroy(tile2.GetGameObject());

			} else {
				tile1.Deselect();
				tile2.Deselect();
			}
			tile1 = null;
			tile2 = null;
		}
	}

	public void TileDeselected(Tile aTile) {
		if (aTile.Equals(tile1))
			tile1 = null;
		else if (aTile.Equals(tile2))
			tile2 = null; 
	}

	// Use this for initialization
	void Awake () {
		board = Board.getInstance();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}