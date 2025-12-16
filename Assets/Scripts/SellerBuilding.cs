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
    [SerializeField] Vector3 ui_offset = new Vector3(300, 0, 100);
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
        Init();
    }
    void Init()
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
        if (ui != null)
            ui.UpdateSlider(value);
        else
        {
            Init();
            ui.UpdateSlider(value);
        }

        if (PlayerController.Instance.currentState != PlayerState.Loading && PlayerController.Instance.selectedBuilding == this)
            if (uiPanel.GetCurrentBuilding() == this)
                uiPanel.UpdateProgressionSlider(value);
    }
    void UpdateUI()
    {

        if (PlayerController.Instance.currentState != PlayerState.Loading)
            ui.UpdateUI(currentResource); 
    }

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
        Vector3 buildingPosition = Camera.main.WorldToScreenPoint(transform.position);
        if ((buildingPosition.x + ui_offset.x + uiRectTransform.rect.width / 2) > Camera.main.pixelWidth)
            uiRectTransform.position = buildingPosition - ui_offset;
        else
            uiRectTransform.position = buildingPosition + ui_offset;
        uiPanel.gameObject.SetActive(true);
        uiPanel.UpdatePanel(currentResource);
        uiPanel.UpdateProgressionSlider(Timer / timeToSale);
    }

    void DeactivateUIPanel() => uiPanel.gameObject.SetActive(false);

    public void ChangeResource(Resource newResource)
    {
        currentResource = newResource;
        UpdateUI();
        if (PlayerController.Instance.currentState != PlayerState.Loading)
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
    public override SaveSystem.PlayerProfile.BuildingInfo ToBuildingInfo()
    {
        SaveSystem.PlayerProfile.BuildingInfo buildingInfo = base.ToBuildingInfo();
        buildingInfo.buildingResource = currentResource;
        buildingInfo.isWorking = isWorking;
        return buildingInfo;
    }
}
