using System;
using UnityEngine;
using UnityEngine.Assertions;
using strange.extensions.context.api;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.impl;

public class MakeComboCommand : Command
{
	[Inject]
	public IComboModel comboModel{ get; set; }
	[Inject]
	public ISkillInitiator skillInitiator{ get; set; }

	public override void Execute()
	{
		int comboSkillId = comboModel.MakeCombo();
		// To trigger a combo, the skill must exist with a valid id
		// if its -1, somethings wrong with the combo detection system
		// in comboModel
		Assert.AreNotEqual(-1, comboSkillId);
		Debug.LogWarning("MakeComboCommand called");
		skillInitiator.InvokeSkillFuncFromSkillId(comboSkillId);
	}
}
