using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.impl;

public class MakeComboCommand : Command
{
	[Inject]
	public IComboModel comboModel{ get; set; }

	public override void Execute()
	{
		comboModel.MakeCombo();
	}
}
