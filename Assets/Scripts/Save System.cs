using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public static class SaveSystem
{
    //  Paths
    public static readonly string[] player_save_paths = { "/saves/save1.json", "/saves/save2.json", "/saves/save3.json" };
    public static readonly string player_setting_path = "/saves/settings.json";
    public static int CurrentPlayerProfileNumber = 0;
    public static PlayerProfile currentPlayerProfile;
    public static UnityEvent onSaveProfile;
    public static UnityEvent onLoadProfile;
    public static PlayerSettings playerSettings;

    public static void LoadProfile()
    {
        string currentDataPath = GetActualPathOfCurrentProfile();
        if (File.Exists(currentDataPath))
        {
            string json = File.ReadAllText(currentDataPath);
            currentPlayerProfile = JsonUtility.FromJson<PlayerProfile>(json);
        }
        else
        {
            currentPlayerProfile = new PlayerProfile();
        }
    }
    public static void SaveProfile()
    {
        string currentDataPath = GetActualPathOfCurrentProfile();
        string jsonSaveData = JsonUtility.ToJson(currentPlayerProfile);
        if(!Directory.Exists(Application.persistentDataPath+"/saves"))
            Directory.CreateDirectory(Application.persistentDataPath + "/saves");

        File.WriteAllText(currentDataPath, jsonSaveData);
    }
    public static void LoadSettings()
    {
        string currentDataPath = GetActualPathOfSettings();
        if (File.Exists(currentDataPath))
        {
            string json = File.ReadAllText(currentDataPath);
            playerSettings = JsonUtility.FromJson<PlayerSettings>(json);
            CurrentPlayerProfileNumber = playerSettings.currentProfileNumber;
        }
        else
        {
            playerSettings = new PlayerSettings(0);
        }
    }
    public static void SaveSettings()
    {
        string currentDataPath = GetActualPathOfSettings();
        string jsonSaveData = JsonUtility.ToJson(new PlayerSettings(CurrentPlayerProfileNumber));
        if (!Directory.Exists(Application.persistentDataPath + "/saves"))
            Directory.CreateDirectory(Application.persistentDataPath + "/saves");

        File.WriteAllText(currentDataPath, jsonSaveData);
    }
    
    //  Методы доступа к полным путям файлов
    public static string GetActualPathOfCurrentProfile()
        => Application.persistentDataPath + player_save_paths[CurrentPlayerProfileNumber];
    public static string GetActualPathOfProfileWithNumber(int profileNumber)
        => Application.persistentDataPath + player_save_paths[profileNumber];
    public static string GetActualPathOfSettings()
        => Application.persistentDataPath + player_setting_path;

    //  Класс данных профиля
    [Serializable]
    public class PlayerProfile
    {
        [Serializable]
        public class BuildingInfo
        {
            public BuildingTypes buildingType;
            public Recipe buildingRecipe;
            public Resource buildingResource;
            public Vector3 buildingPosition;
            public bool isWorking;

            public BuildingInfo() { }
        }

        //  Resources
        public int moneyCount;
        public ResourceStorage resourceStorage;

        //  Buildings
        public List<BuildingInfo> buildingsList;

        //  Quests
        public List<Quest> questsList;
        public int currentQuestLevel;

    }
    
    public class PlayerSettings
    {
        public int currentProfileNumber = 0;
        public PlayerSettings(int profileNumber)
        {
            currentProfileNumber = profileNumber;
        }
    }
}
