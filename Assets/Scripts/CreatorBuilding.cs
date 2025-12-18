using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Player;
using UnityEngine;

public class CreatorBuilding : Building
{
    //  Скрипт отвечает за производящее и перерабатывающее здания
    
    public CreatorBuildingHUD buildingHUD;
    public CreatorBuildingUI buildingUI;

    public Recipe currentRecipe;
    public float buildingPower = 1;

    float productingProgression = 0;
    bool isProducting = false;  //  Выполняется ли какой-либо процесс
    public bool isWorking = true;  //  Работает ли здание в данный момент
    bool wereWorkingBeforeMoving = true;
    [SerializeField] Vector3 ui_offset = new Vector3(300, 0, 100);

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    void Init()
    {
        buildingHUD = GetComponent<CreatorBuildingHUD>();
        buildingHUD.Init(buildingType);
        UpdateHUD();
    }


    // Update is called once per frame
    void Update()
    {
        if (isWorking && currentRecipe.CanBeProduced)
        {
            if (isProducting)
                Production();
            else
                StartProducting();
        }
    }

    /*-----Производство-----*/
    //  Основной метод производства
    void Production()
    {
        if (productingProgression < currentRecipe.RecipeHardness)
        {
            AddProgression(buildingPower * Time.deltaTime);
            CheckProductionCompletion();
        }
        else if (productingProgression >= currentRecipe.RecipeHardness)
            CheckProductionCompletion();
    }
    //  Проверка на завершение производства
    void CheckProductionCompletion()
    {
        //  Если набрали достаточно очков прогрессии для данного ресурса, то выдаём его
        if (productingProgression >= currentRecipe.RecipeHardness)
        {
            if (GameManager.Instance.AddResourceToBank(currentRecipe.OutResource.resource, currentRecipe.OutResource.count))
            {
                StopProducting();
                buildingHUD.SpawnResourceEffect();
            }
        }
    }

    //  Метод для смены рецепта
    public void ChangeRecipe(Recipe newRecipe)
    {
        currentRecipe = newRecipe;
        UpdateHUD();
        if (PlayerController.Instance.currentState != PlayerState.Loading)
            buildingUI.UpdateUI(currentRecipe);
        StopProducting();
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

    /*-----Управление производством-----*/
    void StartProducting()
    {
        if (currentRecipe != null)
        {
            if (currentRecipe.recipeType == RecipeType.ZeroToOne)
            {
                isProducting = true;
            }
            else
            {
                if (GameManager.Instance.TryTakeListOfResourceFromBank(currentRecipe.InResources))
                {
                    isProducting = true;
                }
            }
        }
    }
    void StopProducting()
    {
        isProducting = false;
        ResetProgression();
    }
    public void SwitchProductingState()
    {
        if (isWorking)
            PauseProducting();
        else
            ContinueProducing();
    }
    public void PauseProducting() => isWorking = false;
    public void ContinueProducing() => isWorking = true;

    /*-----Взаимодействие с UI-----*/
    //  Обновление прогресс баров
    void UpdateProgressionSlider()
    {
        float value = productingProgression / currentRecipe.RecipeHardness;
        //  HUD
        buildingHUD.UpdateSlider(value);

        //  Окно здания (если оно открыто)
        if (PlayerController.Instance.currentState != PlayerState.Loading&&PlayerController.Instance.selectedBuilding==this)
            if (buildingUI.GetCurrentBuilding() == this)
                buildingUI.UpdateProgressionSlider(value);
    }
    //  Обновление HUD
    void UpdateHUD()
    {
        if (buildingHUD != null)
            buildingHUD.UpdateUI(currentRecipe);
        else
        {
            Init();
            buildingHUD.UpdateUI(currentRecipe);
        }
    }

    void ActivateUIPanel()
    {
        RectTransform uiRectTransform = (RectTransform)buildingUI.transform;
        Vector3 buildingPosition = Camera.main.WorldToScreenPoint(transform.position);
        if ((buildingPosition.x + ui_offset.x + uiRectTransform.rect.width/2) > Camera.main.pixelWidth)
            uiRectTransform.position = buildingPosition - ui_offset;
        else
            uiRectTransform.position = buildingPosition + ui_offset;
        buildingUI.gameObject.SetActive(true);
        buildingUI.UpdateUI(currentRecipe);
        buildingUI.UpdateProgressionSlider(productingProgression / currentRecipe.RecipeHardness);
    }
    void DeactivateUIPanel()
    {
        buildingUI.gameObject.SetActive(false);
    }

    /*-----Переопределения базовых методов-----*/
    
    //  При выделении зданий также открывается панель
    public override void OnSelect()
    {
        base.OnSelect();
        buildingUI = PlayerController.Instance.creatorBuildingUI;
        buildingUI.SetCurrentBuilding(this);
        ActivateUIPanel();
    }
    public override void OnDeselect()
    {
        base.OnDeselect();
        DeactivateUIPanel();
    }

    public override void DeleteBuilding()
    {
        DeactivateUIPanel();
        base.DeleteBuilding();
    }
    public override void MoveBuilding()
    {
        wereWorkingBeforeMoving = isWorking;
        PauseProducting();
        base.MoveBuilding();
    }
    public override void OnPlaced()
    {
        base.OnPlaced();
        if (wereWorkingBeforeMoving)
            ContinueProducing();
    }

    public override SaveSystem.PlayerProfile.BuildingInfo ToBuildingInfo()
    {
        SaveSystem.PlayerProfile.BuildingInfo buildingInfo = base.ToBuildingInfo();
        buildingInfo.buildingRecipe = currentRecipe;
        buildingInfo.isWorking = isWorking;
        return buildingInfo;
    }
}
