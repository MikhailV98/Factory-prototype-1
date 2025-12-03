using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Player;
using UnityEngine;

public class CreatorBuilding : Building
{
    public CreatorBuildingHUD buildingHUD;
    public CreatorBuildingUI buildingUI;

    public Recipe currentRecipe;
    public float buildingPower = 1;

    float productingProgression = 0;
    bool isProducting = false;  //  Выполняется ли какой-либо процесс
    public bool isWorking = true;  //  Работает ли здание в данный момент
    bool wereWorkingBeforeMoving = true;

    // Start is called before the first frame update
    void Start()
    {
        buildingHUD = GetComponent<CreatorBuildingHUD>();
        UpdateHUD();

        buildingUI = PlayerController.Instance.creatorBuildingUI;
    }

    void StartProducting()
    {
        if (currentRecipe != null)
        {
            if (currentRecipe.recipeType==RecipeType.ZeroToOne)
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
    public void PauseProducting()
    {
        isWorking = false;
    }
    public void ContinueProducing()
    {
        isWorking = true;
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

    void CheckProductionCompletion()
    {
        //  Если набрали достаточно очков прогрессии для данного ресурса, то выдаём его
        if (productingProgression >= currentRecipe.RecipeHardness)
        {
            if (GameManager.Instance.AddResourceToBank(currentRecipe.OutResource.resource, currentRecipe.OutResource.count))
            {
                StopProducting();
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
    void UpdateProgressionSlider()
    {
        float value = productingProgression / currentRecipe.RecipeHardness;
        buildingHUD.UpdateSlider(value);
        if (buildingUI.GetCurrentBuilding() == this)
            buildingUI.UpdateProgressionSlider(value);
    }

    void UpdateHUD() => buildingHUD.UpdateUI(currentRecipe);

    //  При выделении зданий также открывается панель
    public override void OnSelect()
    {
        base.OnSelect();
        buildingUI.SetCurrentBuilding(this);
        ActivateUIPanel();
    }
    public override void OnDeselect()
    {
        base.OnDeselect();
        DeactivateUIPanel();
    }

    void ActivateUIPanel()
    {
        RectTransform uiRectTransform = (RectTransform)buildingUI.transform;
        Vector3 position = Camera.main.WorldToScreenPoint(transform.position) + new Vector3(300,0,100);
        uiRectTransform.position = position;
        buildingUI.gameObject.SetActive(true);
        buildingUI.UpdateUI(currentRecipe);
        buildingUI.UpdateProgressionSlider(productingProgression / currentRecipe.RecipeHardness);
    }
    void DeactivateUIPanel()
    {
        buildingUI.gameObject.SetActive(false);
    }

    public void ChangeRecipe(Recipe newRecipe)
    {
        currentRecipe = newRecipe;
        UpdateHUD();
        buildingUI.UpdateUI(currentRecipe);
        StopProducting();
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
}
