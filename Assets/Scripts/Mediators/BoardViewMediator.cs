using System;
using Eppy;
using EUserInputDataRequests;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;


public class BoardViewMediator : Mediator {

    [Inject]
    public BoardView boardView { get; set; }
    [Inject]
    public IBoardModel boardModel { get; set; }
    [Inject]
    public ISkillInitiator skillInitiator { get; set; }

    [Inject]
    public AttemptTileCancelSignal attemptTileCancelSignal { get; set; }
    [Inject]
    public BoardIsEmptySignal boardIsEmptySignal { get; set; }
    [Inject]
    public ResetActiveStateSignal resetActiveStateSignal { get; set; }
    [Inject]
    public TileDestroyedSignal tileDestroyedSignal { get; set; }
    [Inject]
    public UserInputDataRequestSignal dataRequestSignal { get; set; }
    [Inject]
    public UserInputDataResponseSignal dataResponseSignal { get; set; }

    private Tile tile1;
    private Tile tile2;

    private bool requestingUserInput = false;
    private Enum pendingRequestType = null;

    public void TileSelected(Tile aTile) {
        if (requestingUserInput) {
            ReplyToRequest(aTile);
        } else {
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
    }

    public void TileDeselected(Tile aTile) {
        if (requestingUserInput) {
            ReplyToRequest(aTile);
        } else {
            if (aTile.Equals(tile1))
                tile1 = null;
            else if (aTile.Equals(tile2))
                tile2 = null; 
        }
    }

    public bool BoardIsEmpty() {
        return boardModel.isEmpty();
    }

    public void ResetBoard() {
        boardModel.GenerateBoard();
        boardView.ResetBoard(boardModel);
    }

    public override void OnRegister() {
        boardModel.GenerateBoard();
        boardView.Init(boardModel);

        resetActiveStateSignal.AddListener(ResetBoard);

        boardView.tileSelectedSignal.AddListener(TileSelected);
        boardView.tileDeselectedSignal.AddListener(TileDeselected);

        tileDestroyedSignal.AddListener(boardView.DestroyTile);

        dataRequestSignal.AddListener(ResolveRequest);
    }

    private void ResolveRequest(Enum userInputRequest) {
        if (userInputRequest.GetType().Equals(typeof(BoardViewRequest))) {
            requestingUserInput = true;
            pendingRequestType = userInputRequest;

            if (userInputRequest.Equals(BoardViewRequest.SELECT_COL)) {
                boardView.highlightingColumn = true;
            }
        }
    }

    private void ReplyToRequest(Tile aTile) {
        ActionParams paramsList = new ActionParams();
        if (pendingRequestType.Equals(BoardViewRequest.SELECT_COL)) {
            paramsList.AddToParamList(aTile.Column);
            dataResponseSignal.Dispatch(pendingRequestType, paramsList);
            boardView.highlightingColumn = false;
        }

        requestingUserInput = false;
        pendingRequestType = null;
    }
}
