using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductivityBuilding : Building
{
    //  Класс производящего здания
    ProductivityBuildingUI ui;          //  UI - Вокруг здания (название, иконка ресурса, прогресс бар)
    ProductBuildingPanel uiPanel;       //  UI - Открывающаяся панель

    public Resource productingRecource; //  Производимый ресурс
    public float buildingPower = 1;     //  Сила здания. Определяет, сколько сложностей ресурса создаёт здание за секунду
    float productingProgression = 0;

    bool isProducting = false;

    private void Start()
    {
        ui = GetComponent<ProductivityBuildingUI>();
        UpdateUI();

        uiPanel = PlayerController.Instance.productBuildingPanelUI;
        isProducting = true;

    }
    private void Update()
    {
        Production();
    }
    void Production()
    {
        if (isProducting && (productingProgression < productingRecource.resourceHardness))
        {
            AddProgression(buildingPower * Time.deltaTime);
            CheckProductionCompletion();
        }
        else if(productingProgression >= productingRecource.resourceHardness)
            CheckProductionCompletion();
    }
    void CheckProductionCompletion()
    {
        //  Если набрали достаточно очков прогрессии для данного ресурса, то выдаём его
        if (productingProgression >= productingRecource.resourceHardness)
        {
            if (GameManager.Instance.AddResourceToBank(productingRecource))
            {
                productingProgression = 0;
            }
        }
    }

    //  Прогрессия производства
    void AddProgression(float value)
    {
        productingProgression += value;
        UpdateProgressionSlider();
    }
    void ResetProgression()
    {
        productingProgression = 0;
        UpdateProgressionSlider();
    }
    void UpdateProgressionSlider() => ui.UpdateSlider(productingProgression / productingRecource.resourceHardness);

    void UpdateUI()
    {
        ui.UpdateUI(productingRecource);
    }

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
        uiPanel.UpdatePanel(productingRecource);
    }
    void DeactivateUIPanel()
    {
        uiPanel.gameObject.SetActive(false);
    }

    override public void ChangeResource(Resource newResource)
    {
        productingRecource = newResource;
        UpdateUI();
        uiPanel.UpdatePanel(productingRecource);
        ResetProgression();
    }
    public override void DeleteBuilding()
    {
        DeactivateUIPanel();
        base.DeleteBuilding();
    }
}
