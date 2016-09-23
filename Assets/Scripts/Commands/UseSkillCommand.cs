using System;
using UnityEngine;
using UnityEngine.Assertions;
using strange.extensions.context.api;
using strange.extensions.command.impl;

public class UseSkillCommand : Command
{
	[Inject]
	public IComboModel comboModel{ get; set; }
	[Inject]
	public ISkillInitiator skillInitiator{ get; set; }
	[Inject]
	public int comboId { get; set; }

	private ComboListFetcher fetcher;

	public UseSkillCommand() {
		fetcher = ComboListFetcher.GetInstance();
	}

	public override void Execute()
	{
		// To trigger a combo, the combo must exist with a valid id
		Assert.AreNotEqual(-1, comboId);
		int skillId = fetcher.GetComboSkillIdById(comboId);
		Assert.AreNotEqual(-1, skillId);
		skillInitiator.InvokeSkillFuncFromSkillId(skillId, fetcher.GetComboSkillParamsById(comboId));
		comboModel.DeductEquippedComboElems(comboId);
	}
}
