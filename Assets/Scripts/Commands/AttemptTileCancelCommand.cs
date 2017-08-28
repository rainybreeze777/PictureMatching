using System;
using Eppy;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.impl;

public class AttemptTileCancelCommand : Command
{
    [Inject]
    public IComboModel comboModel{ get; set; }
    [Inject]
    public IBoardModel boardModel{ get; set; }
    [Inject]
    public Tuple<Tile, Tile> cancellingPair{ get; set; }

    private string ElemGatheredPanelTag = "ElemGathered";

    public override void Execute()
    {
        Tile tile1 = cancellingPair.Item1;
        Tile tile2 = cancellingPair.Item2;

        if (boardModel.isRemovable(tile1.Row, tile1.Column, tile2.Row, tile2.Column)) {
            // Order matters here, have to add to combo first
            // Because when boardModel removes the tiles, it will dispatch
            // boardIsEmpty signal, which in turn will immediately initiate
            // battle resolution, but battle resolution will attempt to
            // get cancel sequence from comboModel, which doesn't have
            // the last cancel tile number
            comboModel.AddToCancelSequence(tile1.TileNumber);
            boardModel.remove(tile1.Row, tile1.Column, tile2.Row, tile2.Column);
            GameObject partiSys = Resources.Load("Prefabs/Particles/TileDestroyParticle") as GameObject;
            AcceleratedMoveTowards instance1 = (GameObject.Instantiate(partiSys
                        , tile1.transform.position
                        , Quaternion.identity) as GameObject).GetComponent<AcceleratedMoveTowards>();
            AcceleratedMoveTowards instance2 = (GameObject.Instantiate(partiSys
                        , tile2.transform.position
                        , Quaternion.identity) as GameObject).GetComponent<AcceleratedMoveTowards>();

            GameObject elemGatheredPanel = GameObject.FindWithTag(ElemGatheredPanelTag);
            Vector3 panelPos = Camera.main.ScreenToWorldPoint(elemGatheredPanel.transform.position);

            instance1.StartMove(panelPos, true);
            instance2.StartMove(panelPos, true);
        } else {
            tile1.Deselect();
            tile2.Deselect();
        }
    }
}
