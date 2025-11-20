using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductBuildingDropDown : TMP_Dropdown
{
    //   остыльный дропдаун чтобы при взаимодействии с выпадающим списком не считывались другие нажати€ мышкой

    protected override void Start()
    {
        base.Start();
        FillDropDownList();
    }
    //  јвтозаполнение выпадающего списка всеми ресурсами
    void FillDropDownList()
    {
        List<OptionData> newList = new List<OptionData>();
        foreach (Resource r in GameManager.Instance.gameParams.resourcesList)
        {
            newList.Add(new OptionData(r.Name, r.Image));
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
        PlayerController.Instance.ChangeResourceOnSelectedBuilding(GameManager.Instance.gameParams.resourcesList[value]);
        base.DestroyDropdownList(dropdownList);
    }
}
