using UnityEngine;
using System.Collections;

public class BoardController : MonoBehaviour {

    public ComboController comboController;
    public BoardDisplay boardDisplay;

    private Board board;
    private TileInfoFetcher infoFetcher;
    private const string spritePath = "Sprites/";

    private Tile tile1;
    private Tile tile2;

    public void TileSelected(Tile aTile) {
        if (tile1 == null) {
            tile1 = aTile;
            tile1.GetGameObject().GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(spritePath + infoFetcher.GetInfoFromNumber(tile1.TileNumber, "selectedSprite"));
        } else if (tile2 == null) {
            tile2 = aTile;
        }

        if (tile1 != null && tile2 != null) {
            if (board.isRemovable(tile1.Row, 
                                    tile1.Column, 
                                    tile2.Row, 
                                    tile2.Column)) {
                board.remove(tile1.Row, tile1.Column, tile2.Row, tile2.Column);
                //Tell ComboController to add this removed tile to list of cancelled tiles.
                comboController.AddToCancelSequence(tile1.TileNumber);
                Destroy(tile1.GetGameObject());
                Destroy(tile2.GetGameObject());

            } else {
                tile1.GetGameObject().GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(spritePath + infoFetcher.GetInfoFromNumber(tile1.TileNumber, "normalSprite"));
                tile1.Deselect();
                tile2.Deselect();
            }
            tile1 = null;
            tile2 = null;
        }
    }

    public void TileDeselected(Tile aTile) {
        if (aTile.Equals(tile1)) {
            tile1.GetGameObject().GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(spritePath + infoFetcher.GetInfoFromNumber(tile1.TileNumber, "normalSprite"));
            tile1 = null;
        }
        else if (aTile.Equals(tile2))
            tile2 = null; 
    }

    public bool BoardIsEmpty() {
        return board.isEmpty();
    }

    public void ResetBoard() {
        boardDisplay.ResetBoard();
    }

    // Use this for initialization
    void Awake () {
        board = Board.getInstance();
        infoFetcher = TileInfoFetcher.GetInstance();
    }
    
    // Update is called once per frame
    void Update () {
        
    }
}
