using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIResourceDropdown : TMP_Dropdown
{
    //  Dropdown для выбора ресурсов
    protected override void Start()
    {
        base.Start();
    }
    //  Автозаполнение выпадающего списка всеми ресурсами
    public void FillDropDownList()
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
