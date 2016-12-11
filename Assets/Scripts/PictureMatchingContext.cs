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
        mediationBinder.Bind<SkillHotbarView>().To<SkillHotbarViewMediator>();
        mediationBinder.Bind<EquipmentView>().To<EquipmentViewMediator>();
        mediationBinder.Bind<MapView>().To<MapViewMediator>();
        mediationBinder.Bind<StatusView>().To<StatusViewMediator>();
        mediationBinder.Bind<SceneView>().To<SceneViewMediator>();
        mediationBinder.Bind<PlayerInfoView>().To<PlayerInfoViewMediator>();
        mediationBinder.Bind<SmeltView>().To<SmeltViewMediator>();
        mediationBinder.Bind<InGameMenuView>().To<InGameMenuViewMediator>();
        mediationBinder.Bind<SaveLoadView>().To<SaveLoadViewMediator>();
        // Binding Signals with Commands
        commandBinder.Bind<StartSignal>().To<StartCommand>().Once();
        commandBinder.Bind<AttemptTileCancelSignal>().To<AttemptTileCancelCommand>();
        commandBinder.Bind<EngageCombatSignal>().To<EngageCombatCommand>();
        commandBinder.Bind<ResolveOneExchangeSignal>().To<ResolveOneExchangeCommand>();
        commandBinder.Bind<UseComboSignal>().To<UseComboCommand>();
        commandBinder.Bind<CommenceSmeltSignal>().To<CommenceSmeltCommand>();
        // Dependency Injection Binding
        injectionBinder.Bind<IComboModel>().To<ComboModel>().ToSingleton();
        injectionBinder.Bind<IBoardModel>().To<BoardModel>().ToSingleton();
        injectionBinder.Bind<ISkillInitiator>().To<SkillInitiator>().ToSingleton();
        injectionBinder.Bind<IInBattleStatus>().To<InBattlePlayerStatus>().ToSingleton().ToName(EInBattleStatusType.PLAYER);
        injectionBinder.Bind<IInBattleStatus>().To<InBattleEnemyStatus>().ToSingleton().ToName(EInBattleStatusType.ENEMY);
        injectionBinder.Bind<IEnemyModel>().To<EnemyModel>().ToSingleton();
        injectionBinder.Bind<IBattleResolver>().To<BattleResolver>().ToSingleton();
        injectionBinder.Bind<IPlayerStatus>().To<PlayerStatus>().ToSingleton();
        injectionBinder.Bind<IDialogueParser>().To<DialogueParser>();
        injectionBinder.Bind<ISmeltery>().To<Smeltery>().ToSingleton();
        injectionBinder.Bind<IGameStateMachine>().To<GameStateMachine>().ToSingleton();
        injectionBinder.Bind<ISaverLoader>().To<SaverLoader>().ToSingleton();
        injectionBinder.Bind<IBiographer>().To<PlayerBiographer>().ToSingleton();
        // Instantiating Signals that are triggered manually
        injectionBinder.Bind<BattleResultUpdatedSignal>().ToSingleton();
        injectionBinder.Bind<ComboPossibleSignal>().ToSingleton();
        injectionBinder.Bind<BoardIsEmptySignal>().ToSingleton();
        injectionBinder.Bind<InitiateBattleResolutionSignal>().ToSingleton();
        injectionBinder.Bind<ResetActiveStateSignal>().ToSingleton();
        injectionBinder.Bind<ResetBattleSignal>().ToSingleton();
        injectionBinder.Bind<TileDestroyedSignal>().ToSingleton();
        injectionBinder.Bind<TileRangeDestroyedSignal>().ToSingleton();
        injectionBinder.Bind<UserInputDataRequestSignal>().ToSingleton();
        injectionBinder.Bind<UserInputDataResponseSignal>().ToSingleton();
        injectionBinder.Bind<PlayerHealthUpdatedSignal>().ToSingleton();
        injectionBinder.Bind<EnemyHealthUpdatedSignal>().ToSingleton();
        injectionBinder.Bind<AddToTimeSignal>().ToSingleton();
        injectionBinder.Bind<EnemySeqGenSignal>().ToSingleton();
        injectionBinder.Bind<OneExchangeDoneSignal>().ToSingleton();
        injectionBinder.Bind<ElemGatherUpdatedSignal>().ToSingleton();
        injectionBinder.Bind<SkillExecFinishedSignal>().ToSingleton();
        injectionBinder.Bind<ComboExecFinishedSignal>().ToSingleton();
        injectionBinder.Bind<PlayerEquipComboUpdatedSignal>().ToSingleton();
        injectionBinder.Bind<PlayerEquipWeaponUpdatedSignal>().ToSingleton();
        injectionBinder.Bind<SceneChangeSignal>().ToSingleton();
        injectionBinder.Bind<GameFlowStateChangeSignal>().ToSingleton();
        injectionBinder.Bind<PlayerWeaponsInfoUpdatedSignal>().ToSingleton();
        injectionBinder.Bind<PlayerInfoUpdatedSignal>().ToSingleton();
        injectionBinder.Bind<PlayerEssenceGainedSignal>().ToSingleton();
        injectionBinder.Bind<StatusTabChangedSignal>().ToSingleton();
        injectionBinder.Bind<EscKeyPressedSignal>().ToSingleton();
        injectionBinder.Bind<OpenSaveLoadViewSignal>().ToSingleton();
        injectionBinder.Bind<GameSaveFileOpSignal>().ToSingleton();
        injectionBinder.Bind<SceneLoadFromSaveSignal>().ToSingleton();
        // Manually Instantiate instances and inject
        injectionBinder.GetInstance<ISkillInitiator>().InjectInitialize(injectionBinder);
        // Second Round injection for special circumstances
        injectionBinder.Bind<IInBattleEnemyStatus>().ToValue(injectionBinder.GetInstance<IInBattleStatus>(EInBattleStatusType.ENEMY)).ToSingleton();    
    }
}
