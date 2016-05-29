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
		StartSignal startSignal= (StartSignal)injectionBinder.GetInstance<StartSignal>();
		startSignal.Dispatch();
		return this;
	}

	protected override void mapBindings()
	{
		// Binding Mediators with Views
		mediationBinder.Bind<GameView>().To<GameViewMediator>();
		// Binding Signals with Commands
		commandBinder.Bind<StartSignal>().To<StartCommand>().Once();
	}
}
