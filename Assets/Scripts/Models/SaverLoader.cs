using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;

public class SaverLoader : ISaverLoader {

    [Inject]
    public IPlayerStatus playerStatus { get; set; }
    [Inject]
    public IGameStateMachine gameStateMachine { get; set; }

    [Inject]
    public GameSaveFileOpSignal gameSaveFileOpSignal { get; set; }

    private const int SLOTS_COUNT = 20; 
    public int SlotsCount { get { return SLOTS_COUNT; } }

    [PostConstruct]
    public void PostConstruct() {
        gameSaveFileOpSignal.AddListener(InterpretSignal);
    }

    public void SaveGame(int saveSlotIndex) {
        Debug.Log("Application Data: " + Application.dataPath);

        GameSave save = new GameSave(playerStatus, saveSlotIndex, gameStateMachine.CurrentState);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream saveFile = File.Create(Application.dataPath + "\\Saves\\saveFile" + saveSlotIndex + ".sav");
        bf.Serialize(saveFile, save);
        saveFile.Close();
    }

    public void LoadGame(int loadSlotIndex) {

        GameSave saveToLoad = null;

        if(File.Exists(Application.dataPath + "\\Saves\\saveFile" + loadSlotIndex + ".sav")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream saveFile = File.Open(Application.dataPath + "\\Saves\\saveFile"+ loadSlotIndex + ".sav", FileMode.Open);
            saveToLoad = (GameSave) bf.Deserialize(saveFile);
            saveFile.Close();
        } else {
            throw new ArgumentException("SaverLoader LoadGame() received a load request for non-existing file! Requested load file index: " + loadSlotIndex);
        }

        playerStatus.InitFromGameSave(saveToLoad);

    }

    public List<GameSave> LoadGameSaveFromDisk() {
        List<GameSave> saveFilesList = new List<GameSave>();

        BinaryFormatter bf = new BinaryFormatter();

        for (int i = 0; i < SLOTS_COUNT; ++i) {
            if(File.Exists(Application.dataPath + "\\Saves\\saveFile" + i + ".sav")) {
                FileStream saveFile = File.Open(Application.dataPath + "\\Saves\\saveFile"+ i + ".sav", FileMode.Open);
                saveFilesList.Add( (GameSave) bf.Deserialize(saveFile));
                saveFile.Close();
            }
        }

        return saveFilesList;
    }

    private void InterpretSignal(int slotIndex, EGameSaveFileOp op) {
        switch(op) {
            case EGameSaveFileOp.SAVE:
                SaveGame(slotIndex);
                break;
            case EGameSaveFileOp.LOAD:
                LoadGame(slotIndex);
                break;
            case EGameSaveFileOp.DELETE:
                break;
        }
    }
}
