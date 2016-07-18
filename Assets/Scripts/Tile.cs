using System;
using UnityEngine;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

public class Tile : View {

    public Signal<Tile> selectSignal = new Signal<Tile>();
    public Signal<Tile> deselectSignal = new Signal<Tile>();

    private const string spritePath = "Sprites/";
    private SpriteRenderer spriteRenderer;
    private TileInfoFetcher infoFetcher;

    private bool isSelected = false;

    private int row, column;
    private int tileNumber;

    private Sprite normalSprite;
    private Sprite selectedSprite;

    public int Row { 
        get { return row; }
    }
    public int Column { 
        get { return column; }
    }
    public int TileNumber {
        get { return tileNumber; }
    }

    public Tile() {
        infoFetcher = TileInfoFetcher.GetInstance();
    }

    public void Initialize(int row, int column, int tileNumber) {
        this.row = row;
        this.column = column;
        this.tileNumber = tileNumber;

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        normalSprite = Resources.Load<Sprite>(spritePath + infoFetcher.GetInfoFromNumber(tileNumber, "normalSprite"));
        selectedSprite =  Resources.Load<Sprite>(spritePath + infoFetcher.GetInfoFromNumber(tileNumber, "selectedSprite"));
    }

    public GameObject GetGameObject() {
        return gameObject;
    }

    public void Deselect() {
        isSelected = false;
        spriteRenderer.sprite = normalSprite;
    }

    public bool Equals(Tile otherTile) {
        if ((object)otherTile == null)
            return false;

        return (row == otherTile.Row && column == otherTile.Column);
    }

    public void OnMouseDown() {
        isSelected = !isSelected;

        spriteRenderer.sprite = isSelected ? selectedSprite : normalSprite;

        if(isSelected) {
            // boardController.TileSelected(this);
            selectSignal.Dispatch(this);
        } else {
            // boardController.TileDeselected(this);
            deselectSignal.Dispatch(this);
        }
    }
}
