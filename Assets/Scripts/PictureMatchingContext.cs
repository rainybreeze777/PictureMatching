using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.dispatcher.eventdispatcher.impl;
using strange.extensions.command.api;
using strange.extensions.command.impl;

public class PictureMatchingContext : MVCSContext {

    public PictureMatchingContext (MonoBehaviour view) : base(view)
    {
    }

    public PictureMatchingContext (MonoBehaviour view, ContextStartupFlags flags) : base(view, flags)
    {
    }

    // Unbind the default EventCommandBinder and rebind the SignalCommandBinder
    protected override void addCoreComponents()
    {
        base.addCoreComponents();
        injectionBinder.Unbind<ICommandBinder>();
        injectionBinder.Bind<ICommandBinder>().To<SignalCommandBinder>().ToSingleton();
    }

    // Override Start so that we can fire the StartSignal 
    override public IContext Start()
    {
        base.Start();
        StartSignal startSignal = (StartSignal)injectionBinder.GetInstance<StartSignal>();
        startSignal.Dispatch();
        return this;
    }

    protected override void mapBindings()
    {
        // Binding Mediators with Views
        mediationBinder.Bind<GameView>().To<GameViewMediator>();
        mediationBinder.Bind<BattleView>().To<BattleViewMediator>();
        mediationBinder.Bind<ComboView>().To<ComboViewMediator>();
        mediationBinder.Bind<BoardView>().To<BoardViewMediator>();
        // Binding Signals with Commands
        commandBinder.Bind<StartSignal>().To<StartCommand>().Once();
        commandBinder.Bind<AttemptTileCancelSignal>().To<AttemptTileCancelCommand>();
        commandBinder.Bind<MakeComboSignal>().To<MakeComboCommand>();
        // Dependency Injection Binding
        injectionBinder.Bind<IComboModel>().To<ComboModel>().ToSingleton();
        injectionBinder.Bind<IBoardModel>().To<BoardModel>().ToSingleton();
        injectionBinder.Bind<ISkillInitiator>().To<SkillInitiator>().ToSingleton();
        // Instantiating Signals that are triggered manually
        injectionBinder.Bind<BattleWonSignal>().ToSingleton();
        injectionBinder.Bind<BattleLostSignal>().ToSingleton();
        injectionBinder.Bind<BattleUnresolvedSignal>().ToSingleton();
        injectionBinder.Bind<ComboPossibleSignal>().ToSingleton();
        injectionBinder.Bind<BoardIsEmptySignal>().ToSingleton();
        injectionBinder.Bind<InitiateBattleResolutionSignal>().ToSingleton();
        injectionBinder.Bind<ResetActiveStateSignal>().ToSingleton();
        injectionBinder.Bind<ResetBattleSignal>().ToSingleton();
        injectionBinder.Bind<TileDestroyedSignal>().ToSingleton();
        injectionBinder.Bind<TileRangeDestroyedSignal>().ToSingleton();
        injectionBinder.Bind<UserInputDataRequestSignal>().ToSingleton();
        injectionBinder.Bind<UserInputDataResponseSignal>().ToSingleton();
        // Manuall Instantiate SkillInitiator and inject to skill classes
        injectionBinder.GetInstance<ISkillInitiator>().InjectInitialize(injectionBinder);
    }
}
