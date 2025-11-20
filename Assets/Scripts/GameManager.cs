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
            resourceBank = new ResourceStorage();
        }
        else
        {
            Destroy(this); 
        }
    }
    private void Start()
    {
        resourcePanel.Init(resourceBank);
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
    void UpdateResourceUI()
    {
        moneyText.text = moneyCount.ToString();
    }

    public bool TryTakeResourceFromBank(Resource r) => resourceBank.TakeResource(r, 1);
    public bool TryTakeResourceFromBank(Resource r, int count) => resourceBank.TakeResource(r, count);

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
}