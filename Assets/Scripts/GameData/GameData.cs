using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameDataContainer
/// </summary>
[System.Serializable]
public class GameData
{
    public int DataSlot;
    public int CurrentScene;
    public string SaveTime;
    public GameData(int slot)
    {
        this.DataSlot = slot; // GameSlot to save GameData
        this.CurrentScene = 1; // "Scene01" is next to StartMenuScene
        this.SaveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // Init save time
    }
}
