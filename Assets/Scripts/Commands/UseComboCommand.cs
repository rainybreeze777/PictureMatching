using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using strange.extensions.context.api;
using strange.extensions.command.impl;

public class UseComboCommand : Command
{
    [Inject]
    public IComboModel comboModel{ get; set; }
    [Inject]
    public ISkillInitiator skillInitiator{ get; set; }
    [Inject]
    public int comboId { get; set; }

    private ComboListFetcher fetcher;

    public UseComboCommand() {
        fetcher = ComboListFetcher.GetInstance();
    }

    public override void Execute()
    {
        // To trigger a combo, the combo must exist with a valid id
        Assert.AreNotEqual(-1, comboId);
        int[] skillIds = fetcher.GetComboSkillIdsById(comboId);
        Assert.AreNotEqual(null, skillIds);
        Assert.IsTrue(skillIds.Length > 0);
        // TODO: temporary fetching method
        ActionParams ap = fetcher.GetComboSkillParamsById(comboId);
        List<ActionParams> p = null;
        if (ap != null) {
            p = new List<ActionParams>();
            p.Add(ap);
        }
        skillInitiator.InvokeSkillFuncFromSkillId(comboId, skillIds, p);
    }
}
