using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreatorDropdown : TMP_Dropdown
{
    protected override void Start()
    {
        base.Start();
    }

    //  Автозаполнение выпадающего списка всеми ресурсами
    public void FillDropDownList(BuildingTypes buildingType)
    {
        switch (buildingType)
        {
            case BuildingTypes.Creator:
                {
                    FillDropdownListForCreator();
                    break;
                }
            case BuildingTypes.Connector:
                {
                    FillDropdownListForConnector();
                    break;
                }
        }
    }

    void FillDropdownListForCreator()
    {
        List<OptionData> newList = new List<OptionData>();
        foreach (Recipe r in GameManager.Instance.gameParams.creatorRecipes)
        {
            newList.Add(new OptionData(r.OutResource.resource.Name, r.OutResource.resource.Image));
        }
        options = newList;
    }
    void FillDropdownListForConnector()
    {
        List<OptionData> newList = new List<OptionData>();
        foreach (Recipe r in GameManager.Instance.gameParams.connectorRecipes)
        {
            newList.Add(new OptionData(r.OutResource.resource.Name, r.OutResource.resource.Image));
        }
        options = newList;
    }

    protected override GameObject CreateDropdownList(GameObject template)
    {
        PlayerController.Instance.currentState = PlayerState.ChoosingResourceInPanel;
        return base.CreateDropdownList(template);
    }
    protected override void DestroyDropdownList(GameObject dropdownList)
    {
        PlayerController.Instance.currentState = PlayerState.Selecting;
        PlayerController.Instance.ChangeRecipeOnSelectedBuilding(value);
        base.DestroyDropdownList(dropdownList);
    }
}
