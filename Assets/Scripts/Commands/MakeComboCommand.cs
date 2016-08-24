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

	private ComboListFetcher fetcher;

	public MakeComboCommand() {
		fetcher = ComboListFetcher.GetInstance();
	}

	public override void Execute()
	{
		int comboId = comboModel.MakeCombo();
		// To trigger a combo, the combo must exist with a valid id
		// if its -1, somethings wrong with the combo detection system
		// in comboModel
		Assert.AreNotEqual(-1, comboId);
		int skillId = fetcher.GetComboSkillIdById(comboId);
		Assert.AreNotEqual(-1, skillId);
		skillInitiator.InvokeSkillFuncFromSkillId(skillId, fetcher.GetComboSkillParamsById(comboId));
	}
}
