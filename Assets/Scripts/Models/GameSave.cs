using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSave {

    [SerializeField] private int slotIndex;
    public int SlotIndex { get { return slotIndex; } }
    [SerializeField] private int health;
    public int Health { get { return health; } }
    [SerializeField] private int damage;
    public int Damage { get { return damage; } }
    [SerializeField] private List<int> essence;
    public List<int> Essence { get { return essence; } }
    [SerializeField] private List<int> weaponsInPossession;
    public List<int> WeaponsInPossession { get { return weaponsInPossession; } }
    [SerializeField] private List<int> equippedWeapons;
    public List<int> EquippedWeapons { get { return equippedWeapons; } }
    [SerializeField] private DateTime saveTime;
    public string GetSaveTime() { return saveTime.ToString(DATE_FORMAT); }
    [SerializeField] private EGameFlowState state = EGameFlowState.MAP; // default to MAP state
    public EGameFlowState GameState { get { return state; } }

    private static string DATE_FORMAT = "yyyy/MM/dd";
    public static string GetDateFormat() { return DATE_FORMAT; }

    public GameSave(IPlayerStatus status, int index, EGameFlowState gameState) {
        saveTime = DateTime.Now;
        slotIndex = index;

        if (gameState != EGameFlowState.MAP
            && gameState != EGameFlowState.STATUS
            && gameState != EGameFlowState.SCENE) {
            throw new ArgumentException("GameSave constructor received an invalid save state: " + gameState);
        } else if (gameState == EGameFlowState.STATUS) {
            state = EGameFlowState.MAP;
        } else {
            state = gameState;
        }

        health = status.Health;
        damage = status.Damage;
        essence = new List<int>();
        essence.Add(status.MetalEssence);
        essence.Add(status.WoodEssence);
        essence.Add(status.WaterEssence);
        essence.Add(status.FireEssence);
        essence.Add(status.EarthEssence);
        weaponsInPossession = new List<int>();
        List<Weapon> gameWeapons = status.GetPossessedWeapons();
        foreach(Weapon w in gameWeapons) {
            weaponsInPossession.Add(w.ID);
        }
        equippedWeapons = new List<int>();
        List<Weapon> gameEquipped = status.GetEquippedWeapons();
        foreach(Weapon w in gameEquipped) {
            equippedWeapons.Add(w.ID);
        }
    }
}
