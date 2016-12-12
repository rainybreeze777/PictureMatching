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
    public TileDestroyedSignal tileDestroyedSignal { get; set; }
    [Inject]
    public TileRangeDestroyedSignal tileRangeDestroyedSignal { get; set; }
    [Inject]
    public UserInputDataRequestSignal dataRequestSignal { get; set; }
    [Inject]
    public UserInputDataResponseSignal dataResponseSignal { get; set; }
    [Inject]
    public NewBoardConstructedSignal newBoardConstructedSignal { get; set; }

    private Tile tile1;
    private Tile tile2;

    private bool requestingUserInput = false;
    private Enum pendingRequestType = null;

    public override void OnRegister() {
        boardView.Init();

        boardView.tileSelectedSignal.AddListener(TileSelected);
        boardView.tileDeselectedSignal.AddListener(TileDeselected);
        boardView. inputCancelledSignal.AddListener(OnInputCancelled);

        tileDestroyedSignal.AddListener(boardView.DestroyTile);
        tileRangeDestroyedSignal.AddListener(DestroyTileRange);

        dataRequestSignal.AddListener(ResolveRequest);

        newBoardConstructedSignal.AddListener(OnNewBoardConstructed);
    }

    public void TileSelected(Tile aTile) {
        if (requestingUserInput) {
            boardView.ClearHighlightedStatus();
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

    private void ResolveRequest(Enum userInputRequest) {
        if (userInputRequest.GetType().Equals(typeof(BoardViewRequest))) {
            requestingUserInput = true;
            pendingRequestType = userInputRequest;

            if (userInputRequest.Equals(BoardViewRequest.SELECT_COL)) {
                boardView.EnableHighlightColumn();
            } else if (userInputRequest.Equals(BoardViewRequest.SELECT_SQUARE2)) {
                boardView.EnableHighlightArea(2, 2);
            }
        }
    }

    private void ReplyToRequest(Tile aTile) {
        ActionParams paramsList = new ActionParams();

        if (pendingRequestType.Equals(BoardViewRequest.SELECT_COL)) {
            paramsList.AddToParamList(aTile.Column);
            boardView.DisableHighlightingColumn();
        } else if (pendingRequestType.Equals(BoardViewRequest.SELECT_SQUARE2)) {
            paramsList.AddToParamList(boardView.SelectedAreaModelRowStart);
            paramsList.AddToParamList(boardView.SelectedAreaModelRowEnd);
            paramsList.AddToParamList(boardView.SelectedAreaModelColStart);
            paramsList.AddToParamList(boardView.SelectedAreaModelColEnd);
            boardView.DisableHighlightArea();
        }

        dataResponseSignal.Dispatch(pendingRequestType, paramsList);

        requestingUserInput = false;
        pendingRequestType = null;
    }

    private void DestroyTileRange(
        int rowStart
        , int rowEnd
        , int colStart
        , int colEnd)
    {
        for (int r = rowStart; r <= rowEnd; ++r) {
            for (int c = colStart; c <= colEnd; ++c) {
                boardView.DestroyTile(r, c);
            }
        }
    }

    private void OnInputCancelled() {
        requestingUserInput = false;
        pendingRequestType = null;
        dataResponseSignal.Dispatch(ESkillStatus.INPUT_CANCELLED, null);
    }

    private void OnNewBoardConstructed() {
        boardView.BoardSetup(boardModel);
    }
}
