using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Framework
{
    /// <summary>
    /// Game Data Manager, providing external methods for loading game data.
    /// </summary>
    public class GameDataManager : Singleton<GameDataManager>
    {
        private GameDataManager() { }

        private string savingPath
        {
            get
            {
                return Application.persistentDataPath + "/save/";
            }
        }
        public string SavingPath
        {
            get
            {
                return GetSavePath(0000000);
            }
        }

        private static GameData currentGame;
        public static GameData CurrentGame
        {
            get
            {
                if (currentGame != null)
                {
                    return currentGame;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get the save file path.
        /// </summary>
        /// <param name="slot">Save slot number.</param>
        /// <returns>File path of the save slot.</returns>
        private string GetSavePath(int slot)
        {
            return $"{savingPath}save_slot_{slot}.json";
        }

        /// <summary>
        /// Save game data.
        /// </summary>
        /// <param name="data">Game data to be saved.</param>
        /// <param name="slot">Save slot number.</param>
        public void SaveGame(GameData data, int slot)
        {
            string path = GetSavePath(slot);
            string directory = Path.GetDirectoryName(path);

            // Create the save directory if it does not exist
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonUtility.ToJson(data, true);
            data.SaveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// Loads the game data from a specified save slot.
        /// </summary>
        /// <param name="slot">Save slot number.</param>
        /// <param name="savingCurrentGameData">If true, sets the loaded data as the current game.</param>
        /// <returns>Returns the loaded GameData if the save file exists; otherwise, returns null.</returns>
        public GameData LoadGame(int slot,bool savingCurrentGameData = false)
        {
            string path = GetSavePath(slot);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                GameData data = JsonUtility.FromJson<GameData>(json);

                if (savingCurrentGameData)
                    currentGame = data;

                return data;
            }
            else
            {
                Debug.LogWarning($"Save {slot} does not exist.");
                return null;
            }
        }
        /// <summary>
        /// Delete game data.
        /// </summary>
        /// <param name="slot">Save slot number.</param>
        public void DeleteGame(int slot)
        {
            string path = GetSavePath(slot);
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log($"Save {slot} has been deleted.");
            }
            else
            {
                Debug.LogWarning($"Save {slot} does not exist.");
            }
        }
    }

}
