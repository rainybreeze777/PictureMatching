using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.impl;

public class StartCommand : Command
{
    
    [Inject(ContextKeys.CONTEXT_VIEW)]
    public GameObject contextView{get;set;}
    
    public override void Execute()
    {
        // GameObject gameViewObject = new GameObject();
        // gameViewObject.name = "GameView";
        // gameViewObject.AddComponent<GameView>();
        // gameViewObject.transform.parent = contextView.transform;
    
        GameObject battleViewObject = new GameObject();
        battleViewObject.name = "BattleView";
        battleViewObject.AddComponent<BattleView>();
        battleViewObject.transform.parent = contextView.transform;
    
        GameObject boardViewObject = new GameObject();
        boardViewObject.name = "BoardView";
        boardViewObject.AddComponent<BoardView>();
        boardViewObject.transform.parent = contextView.transform;
    
        GameObject comboViewObject = new GameObject();
        comboViewObject.name = "ComboView";
        comboViewObject.AddComponent<ComboView>();
        comboViewObject.transform.parent = contextView.transform;
    }
}
