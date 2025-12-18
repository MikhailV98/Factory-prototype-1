using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    //  Скрипт для взаимодействия объектов
    //  Также предоставляет необходимые доступы

    public static GameManager Instance; //  Singleton для доступа при необходимости
    public GameParams gameParams;   //  Конфиг основных параметров


    [SerializeField] TextMeshProUGUI moneyText;
    public UIResourcePanel resourcePanel;   //  Ссылка на панель ресурсов

    [SerializeField] int moneyCount = 0;
    [SerializeField] ResourceStorage resourceBank;  //  Хранилище ресурсов
    [SerializeField] int resourceBankCapacity = 50; //  Максимальная вместимость хранилища ресурсов

    AudioSource audioSource;    //  Источник звуков, вызываемых кодом

    //  РЕАЛИЗОВАТЬ АВТОСЕЙВ
    public static UnityEvent onAutoSave = new UnityEvent();

    void Awake()
    {
        //  Реализация Singleton и загрузка профиля
        if (Instance == null)
        {
            Instance = this;
            SaveSystem.LoadProfile();
            InitializeResourceStorage();
            moneyCount = SaveSystem.currentPlayerProfile.moneyCount;
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(this); 
        }
    }
    //  Инициализация хранилища ресурсов
    void InitializeResourceStorage()
    {
        //  Либо берём из сохранения, либо создаём новый
        if (SaveSystem.currentPlayerProfile.resourceStorage != null)
        {
            resourceBank = SaveSystem.currentPlayerProfile.resourceStorage;
        }
        else
        {
            resourceBank = new ResourceStorage(resourceBankCapacity);
            SaveSystem.currentPlayerProfile.resourceStorage = resourceBank;
        }
    }

    private void Start()
    {
        //  Инициализация панели ресурсов и системы квестов
        resourcePanel.Init(resourceBank);
        UpdateResourceUI();
        QuestSystem.Instance.Init();
    }

    //  Метод для начисления ресурса. Возвращает булевый результат - был ли ресурс передан в банк
    public bool AddResourceToBank(Resource resource)
    {
        if (resourceBank.GetTotalCount() < resourceBankCapacity)
        {
            resourceBank.AddResource(resource, 1);
            return true;
        }
        else return false;
    }
    public bool AddResourceToBank(Resource resource, int count)
    {
        if (resourceBank.GetTotalCount() < resourceBankCapacity)
        {
            resourceBank.AddResource(resource, count);
            return true;
        }
        else return false;
    }

    //  Обновление счётчика денег
    void UpdateResourceUI()
    {
        moneyText.text = moneyCount.ToString();
        SaveSystem.currentPlayerProfile.moneyCount = moneyCount;
    }

    //  Методы проверки возможности забора определённых ресурсов из банка
    //  - Возвращение true означает, что ресурсы УЖЕ списаны
    public bool TryTakeResourceFromBank(Resource r) => resourceBank.TakeResource(r, 1);
    public bool TryTakeResourceFromBank(Resource r, int count) => resourceBank.TakeResource(r, count);
    public bool TryTakeListOfResourceFromBank(List<Recipe.RecipeResource> resources)
    {
        //  Проверяем, все ли ресурсы на месте
        bool b = true;
        foreach(Recipe.RecipeResource r in resources)
        {
            if (!resourceBank.HasResource(r.resource, r.count))
                b = false;
        }
        //  Если все ресурсы есть, забираем их
        if (b)
        {
            foreach (Recipe.RecipeResource r in resources)
                resourceBank.TakeResource(r.resource, r.count);
        }
        return b;
    }
    public bool TryTakeListOfResourceFromBank(List<Quest.QuestItem> resources)
    {
        //  Проверяем, все ли ресурсы на месте
        bool b = true;
        foreach (Quest.QuestItem r in resources)
        {
            if (!resourceBank.HasResource(r.resource, r.count))
                b = false;
        }
        //  Если все ресурсы есть, забираем их
        if (b)
        {
            foreach (Quest.QuestItem r in resources)
                resourceBank.TakeResource(r.resource, r.count);
        }
        return b;
    }

    //  Получение количества определённых типов ресурсов
    public int GetResourceCountFromBank(Resource r) => resourceBank.GetResourceCount(r);

    //  Изменение количества денег
    public void AddMoney(int newMoney)
    {
        moneyCount += newMoney;
        UpdateResourceUI();
    }

    //  Поиск положения определённого ресурса в общем списке ресурсов (для Dropdown продающего здания)
    public static int FindCurrentResourceNumberByName(string name)
    {
        List<Resource> resourceList = Instance.gameParams.resourcesList;
        for (int i = 0; i < resourceList.Count; i++)
            if (resourceList[i].Name == name)
                return i;
        return 0;
    }

    //  Получение общего количества зданий данного типа
    public int GetBuildingsCount(BuildingTypes buildingType)
    {
        //  TODO Реализовать подсчёт количества зданий определённого типа
        Building[] buildingArr = GameObject.FindObjectsByType<Building>(FindObjectsSortMode.None);
        int count=0;
        foreach (Building building in buildingArr)
            if (building.buildingType == buildingType)
                count++;
        return count;
    }
    
    //  Проверка, хватает ли у нас денег для постройки здания указанного типа
    public bool TryBuild(BuildingTypes buildingType)
    {
        int cost = GameMath.GetBuildingCost(GetBuildingObjectOfType(buildingType), GetBuildingsCount(buildingType));
        if (cost <= moneyCount)
        {
            AddMoney(-cost);
            return true;
        }
        else
            return false;
    }

    //  Получение объекта здания по его типу
    public BuildingObject GetBuildingObjectOfType(BuildingTypes buildingType)
    {
        foreach (BuildingObject buildingObject in gameParams.buildingsList)
            if (buildingObject.Prefab.GetComponent<Building>().buildingType == buildingType)
                return buildingObject;

        Debug.Log("Can't find BuildingObject of type " + buildingType);
        return null;
    }

    //  Возврат стоимости при отмене строительства здания
    public void ReturnCost(Building building)
        => AddMoney(GameMath.GetBuildingCost(GetBuildingObjectOfType(building.buildingType), GetBuildingsCount(building.buildingType) - 1));

    //  Проигрывание звука
    public void PlaySound(AudioClip sound)
    {
        audioSource.clip = sound;
        audioSource.Play();
    }
}