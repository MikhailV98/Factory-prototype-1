using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellerBuilding : Building
{
    //  Здание продаёт ресурсы с заданой скоростью
    SellerBuildingPanel uiPanel;
    SellerBuildingUI ui;

    public Resource currentResource;
    [SerializeField] float resourcesPerSecond = 2;
    float timeToSale; //  Скорость продажи ресурсов
    float timer = 0;
    public float Timer
    {
        get => timer;
        set
        {
            timer = value;
            UpdateProgressionSlider();
        }
    }
    bool isSelling = false;

    private void Start()
    {
        ui = GetComponent<SellerBuildingUI>();
        UpdateUI();

        uiPanel = PlayerController.Instance.sellerBuildingPanelUI;
        timeToSale = 1 / resourcesPerSecond;
    }

    private void Update()
    {
        if (isSelling)
            Selling();
        else
            CheckIsSellingAvailable();
    }
    void CheckIsSellingAvailable()
    {
        isSelling = GameManager.Instance.TryTakeResourceFromBank(currentResource);
    }
    void Selling()
    {
        if (isSelling && (Timer < timeToSale))
        {
            AddTime(Time.deltaTime);
            CheckSellingCompletion();
        }
        else if (Timer >= timeToSale)
            CheckSellingCompletion();
    }
    void AddTime(float addingTime)
    {
        Timer += addingTime;
    }
    void CheckSellingCompletion()
    {
        //  Если прошло достаточно времени, то продаём ресурс
        if (Timer >= timeToSale)
        {
            GameManager.Instance.AddMoney(currentResource.Cost);
            Timer = 0;
            isSelling = false;
        }
    }
    void ResetTimer()
    {
        Timer = 0;
    }
    void UpdateProgressionSlider() => ui.UpdateSlider(Timer / timeToSale);
    void UpdateUI() => ui.UpdateUI(currentResource);

    //  При выделении зданий также открывается панель
    public override void OnSelect()
    {
        base.OnSelect();
        ActivateUIPanel();
    }
    public override void OnDeselect()
    {
        base.OnDeselect();
        DeactivateUIPanel();
    }

    void ActivateUIPanel()
    {
        uiPanel.gameObject.SetActive(true);
        uiPanel.UpdatePanel(currentResource);
    }

    void DeactivateUIPanel() => uiPanel.gameObject.SetActive(false);

    override public void ChangeResource(Resource newResource)
    {
        currentResource = newResource;
        UpdateUI();
        uiPanel.UpdatePanel(currentResource);
        ResetTimer();
    }

    public override void DeleteBuilding()
    {
        DeactivateUIPanel();
        base.DeleteBuilding();
    }
}
