using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellerBuilding : Building
{
    //  Здание продаёт ресурсы с заданой скоростью
    SellerBuildingPanel uiPanel;
    SellerBuildingUI ui;

    public Resource currentResource;
    [SerializeField] float resourcesPerSecond = 2; //  Скорость продажи ресурсов
    float timeToSale = 1;
    float timer = 0;
    public bool isWorking = true;
    bool wereWorkingBeforeMoving = true;
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

        timeToSale = 1 / resourcesPerSecond;
    }

    private void Update()
    {
        if (isWorking && currentResource.CanBeStored)
        {
            if (isSelling)
                Selling();
            else
                CheckIsSellingAvailable();
        }
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
    void UpdateProgressionSlider()
    {
        float value = Timer / timeToSale;
        ui.UpdateSlider(value);
        if (uiPanel.GetCurrentBuilding() == this)
            uiPanel.UpdateProgressionSlider(value);
    }
    void UpdateUI() => ui.UpdateUI(currentResource);

    //  При выделении зданий также открывается панель
    public override void OnSelect()
    {
        base.OnSelect();
        uiPanel = PlayerController.Instance.sellerBuildingPanelUI;
        uiPanel.SetCurrentBuilding(this);
        ActivateUIPanel();
    }
    public override void OnDeselect()
    {
        base.OnDeselect();
        DeactivateUIPanel();
    }

    void ActivateUIPanel()
    {
        RectTransform uiRectTransform = (RectTransform)uiPanel.transform;
        Vector3 position = Camera.main.WorldToScreenPoint(transform.position) + new Vector3(300, 0, 100);
        uiRectTransform.position = position;
        uiPanel.gameObject.SetActive(true);
        uiPanel.UpdatePanel(currentResource);
        uiPanel.UpdateProgressionSlider(Timer / timeToSale);
    }

    void DeactivateUIPanel() => uiPanel.gameObject.SetActive(false);

    public void ChangeResource(Resource newResource)
    {
        currentResource = newResource;
        UpdateUI();
        uiPanel.UpdatePanel(currentResource);
        ResetTimer();
    }
    public void SwitchSellingState()
    {
        if (isWorking)
            PauseSelling();
        else
            ContinueSelling();
    }
    public void PauseSelling()
    {
        isWorking = false;
    }
    public void ContinueSelling()
    {
        isWorking = true;
    }

    public override void DeleteBuilding()
    {
        DeactivateUIPanel();
        base.DeleteBuilding();
    }

    public override void MoveBuilding()
    {
        wereWorkingBeforeMoving = isWorking;
        PauseSelling();
        base.MoveBuilding();
    }
    public override void OnPlaced()
    {
        base.OnPlaced();
        if (wereWorkingBeforeMoving)
            ContinueSelling();
    }
}
