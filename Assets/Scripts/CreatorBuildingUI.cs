using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreatorBuildingUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] CreatorDropdown dropdown;
    [SerializeField] UIRecipeDisplayer creatorRecipeDisplayer;
    [SerializeField] UIRecipeDisplayer connectorRecipeDisplayer;
    [SerializeField] TextMeshProUGUI pauseButtonText;
    [SerializeField] UIImageProgressBar creatorImageProgressBar;
    [SerializeField] UIImageProgressBar connectorImageProgressBar;
    CreatorBuilding currentBuilding;

    //  Поднятие и снятие флажков при взаимодействии пользователя с панелью
    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayerController.Instance.isMouseOnUI = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        PlayerController.Instance.isMouseOnUI = false;
    }

    public void SetCurrentBuilding(CreatorBuilding newBuilding)
        => currentBuilding = newBuilding;
    public Building GetCurrentBuilding() => currentBuilding;
    

    public void UpdateUI(Recipe recipe)
    {
        nameText.text = currentBuilding.buildingType.ToString();
        dropdown.FillDropDownList(currentBuilding.buildingType, recipe);
        UpdateRecipeDisplayer(recipe);
        UpdatePauseButton();
    }
    public void UpdateProgressionSlider(float value)
    {
        switch(currentBuilding.buildingType)
        {
            case BuildingTypes.Creator:
                {
                    creatorImageProgressBar.ProgressionValue = value;
                    break;
                }
            case BuildingTypes.Connector:
                {
                    connectorImageProgressBar.ProgressionValue = value;
                    break;
                }
        }
    }
    void UpdatePauseButton()
    {
        if (currentBuilding.isWorking)
            pauseButtonText.text = "Pause";
        else pauseButtonText.text = "Continue";
    }
    void UpdateRecipeDisplayer(Recipe recipe)
    {
        switch (currentBuilding.buildingType)
        {
            case BuildingTypes.Creator:
                {
                    connectorRecipeDisplayer.gameObject.SetActive(false);
                    creatorRecipeDisplayer.gameObject.SetActive(true);

                    creatorRecipeDisplayer.SetCurrentRecipe(recipe);
                    creatorRecipeDisplayer.Visualize();
                    break;
                }
            case BuildingTypes.Connector:
                {
                    connectorRecipeDisplayer.gameObject.SetActive(true);
                    creatorRecipeDisplayer.gameObject.SetActive(false);

                    connectorRecipeDisplayer.SetCurrentRecipe(recipe);
                    connectorRecipeDisplayer.Visualize();
                    break;
                }
        }
    }
    public void SwitchStateOnCurrentBuilding()
    {
        currentBuilding.SwitchProductingState();
        UpdatePauseButton();
    }
    public void MoveBuildingClick()
    {
        currentBuilding.MoveBuilding();
        PlayerController.Instance.isMouseOnUI = false;
    }
}
