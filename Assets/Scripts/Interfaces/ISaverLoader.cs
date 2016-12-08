using System;
using System.Collections.Generic;

public interface ISaverLoader {

    int SlotsCount { get; }
    void SaveGame(int saveSlotIndex);
    void LoadGame(int loadSlotIndex);
    List<GameSave> LoadGameSaveFromDisk();

}
