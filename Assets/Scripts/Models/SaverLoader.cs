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
    public IBiographer playerBiographer { get; set; }

    [Inject]
    public GameSaveFileOpSignal gameSaveFileOpSignal { get; set; }
    [Inject]
    public SceneLoadFromSaveSignal sceneLoadFromSaveSignal { get; set; }
    [Inject]
    public GameFlowStateChangeSignal gameFlowStateChangeSignal { get; set; }
    [Inject]
    public AvailableScenesUpdateSignal availableScenesUpdateSignal { get; set; }

    private const int SLOTS_COUNT = 20; 
    public int SlotsCount { get { return SLOTS_COUNT; } }

    private string savePath;

    [PostConstruct]
    public void PostConstruct() {
        savePath = Application.dataPath + "/Saves/";
        gameSaveFileOpSignal.AddListener(InterpretSignal);
    }

    public void SaveGame(int saveSlotIndex) {
        Debug.Log("Save path: " + savePath);

        System.IO.Directory.CreateDirectory(savePath);

        GameSave save = new GameSave(playerStatus, playerBiographer, saveSlotIndex, gameStateMachine.CurrentState, gameStateMachine.CurrentScene);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream saveFile = File.Create(savePath + "saveFile" + saveSlotIndex + ".sav");
        bf.Serialize(saveFile, save);
        saveFile.Close();
    }

    public void LoadGame(int loadSlotIndex) {

        GameSave saveToLoad = null;

        System.IO.Directory.CreateDirectory(savePath);

        if(File.Exists(savePath + "saveFile" + loadSlotIndex + ".sav")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream saveFile = File.Open(savePath + "saveFile"+ loadSlotIndex + ".sav", FileMode.Open);
            saveToLoad = (GameSave) bf.Deserialize(saveFile);
            saveFile.Close();
        } else {
            throw new ArgumentException("SaverLoader LoadGame() received a load request for non-existing file! Requested load file index: " + loadSlotIndex);
        }

        playerStatus.InitFromGameSave(saveToLoad);
        playerBiographer.InitFromGameSave(saveToLoad);
        availableScenesUpdateSignal.Dispatch(-1, EAvailScenesUpdateType.BATCH_UPDATE);
        gameFlowStateChangeSignal.Dispatch(saveToLoad.GameState);
        if (saveToLoad.GameState == EGameFlowState.SCENE) {
            sceneLoadFromSaveSignal.Dispatch(saveToLoad.GameScene);
        }
    }

    public List<GameSave> LoadGameSaveFromDisk() {
        List<GameSave> saveFilesList = new List<GameSave>();

        BinaryFormatter bf = new BinaryFormatter();

        System.IO.Directory.CreateDirectory(savePath);

        for (int i = 0; i < SLOTS_COUNT; ++i) {
            if(File.Exists(savePath + "saveFile" + i + ".sav")) {
                FileStream saveFile = File.Open(savePath + "saveFile"+ i + ".sav", FileMode.Open);
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
