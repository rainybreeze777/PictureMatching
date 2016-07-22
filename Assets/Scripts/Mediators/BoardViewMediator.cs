using System;
using Eppy;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

public class BoardViewMediator : Mediator {

    [Inject]
    public BoardView boardView { get; set; }
    [Inject]
    public IBoardModel boardModel { get; set; }

    [Inject]
    public AttemptTileCancelSignal attemptTileCancelSignal { get; set; }
    [Inject]
    public BoardIsEmptySignal boardIsEmptySignal { get; set; }
    [Inject]
    public ResetActiveStateSignal resetActiveStateSignal { get; set; }
    [Inject]
    public TileDestroyedSignal tileDestroyedSignal { get; set; }

    private Tile tile1;
    private Tile tile2;

    public void TileSelected(Tile aTile) {
        if (tile1 == null) {
            tile1 = aTile;
        } else if (tile2 == null) {
            tile2 = aTile;
        }

        if (tile1 != null && tile2 != null) {

            Tuple<Tile, Tile> attemptToCancelPair = Tuple.Create(tile1, tile2);
            attemptTileCancelSignal.Dispatch(attemptToCancelPair);

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

    public bool BoardIsEmpty() {
        return boardModel.isEmpty();
    }

    public void ResetBoard() {
        boardModel.GenerateBoard();
        boardView.ResetBoard(boardModel);
    }

    public void EnableHighlightCol(bool enable) {
        boardView.highlightingColumn = enable;
    }

    public override void OnRegister() {
        boardModel.GenerateBoard();
        boardView.Init(boardModel);

        resetActiveStateSignal.AddListener(ResetBoard);

        boardView.tileSelectedSignal.AddListener(TileSelected);
        boardView.tileDeselectedSignal.AddListener(TileDeselected);

        tileDestroyedSignal.AddListener(boardView.DestroyTile);
    }
}
