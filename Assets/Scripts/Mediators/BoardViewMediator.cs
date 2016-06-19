using System;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

public class BoardViewMediator : Mediator {

    [Inject]
    public BoardView boardView { get; set; }
    [Inject]
    public IBoardModel boardModel { get; set; }

    [Inject]
    public TileCancelledSignal tileCancelledSignal { get; set; }
    [Inject]
    public BoardIsEmptySignal boardIsEmptySignal { get; set; }
    [Inject]
    public ResetActiveStateSignal resetActiveStateSignal { get; set; }

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
            if (boardModel.isRemovable(tile1.Row, 
                                        tile1.Column, 
                                        tile2.Row, 
                                        tile2.Column)) {

                tileCancelledSignal.Dispatch(tile1.TileNumber);
                Destroy(tile1.GetGameObject());
                Destroy(tile2.GetGameObject());
                
                boardModel.remove(tile1.Row, tile1.Column, tile2.Row, tile2.Column);
                if (boardModel.isEmpty()) {
                    Debug.Log("Dispatching boardIsEmptySignal");
                    boardIsEmptySignal.Dispatch();
                }

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
        return boardModel.isEmpty();
    }

    public void ResetBoard() {
        boardModel.GenerateBoard();
        boardView.ResetBoard(boardModel);
    }

    public override void OnRegister() {
        infoFetcher = TileInfoFetcher.GetInstance();
        boardModel.GenerateBoard();
        boardView.Init(boardModel);

        resetActiveStateSignal.AddListener(ResetBoard);

        boardView.tileSelectedSignal.AddListener(TileSelected);
        boardView.tileDeselectedSignal.AddListener(TileDeselected);
    }
}
