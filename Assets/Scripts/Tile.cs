using System;
using UnityEngine;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

public class Tile : View {

    // private BoardController boardController;
    private bool isSelected = false;

    private int row, column;
    private int tileNumber;

    public Signal<Tile> selectSignal = new Signal<Tile>();
    public Signal<Tile> deselectSignal = new Signal<Tile>();

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

    public void OnMouseDown() {
        isSelected = !isSelected;

        if(isSelected) {
            // boardController.TileSelected(this);
            selectSignal.Dispatch(this);
        } else {
            // boardController.TileDeselected(this);
            deselectSignal.Dispatch(this);
        }
    }
}
