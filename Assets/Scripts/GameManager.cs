using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //  Скрипт для взаимодействия объектов
    //  Также предоставляет необходимые доступы

    public static GameManager Instance;
    public GameParams gameParams;   //  Конфиг основных параметров

    [SerializeField] TextMeshProUGUI moneyText;
    public UIResourcePanel resourcePanel;

    [SerializeField] int moneyCount = 0;
    [SerializeField] ResourceStorage resourceBank;
    [SerializeField] int resourceBankCapacity = 50;

    // Start is called before the first frame update
    void Awake()
    {
        //  Реализация Singleton
        if (Instance == null)
        {
            Instance = this;
            resourceBank = new ResourceStorage(resourceBankCapacity);
        }
        else
        {
            Destroy(this); 
        }
    }
    private void Start()
    {
        resourcePanel.Init(resourceBank);
        UpdateResourceUI();
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

    void UpdateResourceUI()
    {
        moneyText.text = moneyCount.ToString();
    }

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

    public void AddMoney(int newMoney)
    {
        moneyCount += newMoney;
        UpdateResourceUI();
    }

    public static int FindCurrentResourceNumberByName(string name)
    {
        List<Resource> resourceList = Instance.gameParams.resourcesList;
        for (int i = 0; i < resourceList.Count; i++)
            if (resourceList[i].Name == name)
                return i;
        return 0;
    }
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
    public BuildingObject GetBuildingObjectOfType(BuildingTypes buildingType)
    {
        foreach (BuildingObject buildingObject in gameParams.buildingsList)
            if (buildingObject.Prefab.GetComponent<Building>().buildingType == buildingType)
                return buildingObject;

        Debug.Log("Can't find BuildingObject of type " + buildingType);
        return null;
    }

    public void ReturnCost(Building building)
        => AddMoney(GameMath.GetBuildingCost(GetBuildingObjectOfType(building.buildingType), GetBuildingsCount(building.buildingType) - 1));
}