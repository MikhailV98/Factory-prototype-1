using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProductBuildingPanel : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    //  Скрипт взаимодействия с панелью производящего здания
    [SerializeField] Image resourceImage;
    [SerializeField] TextMeshProUGUI resourceText;
    [SerializeField] ProductBuildingDropDown dropdown;

    //  Костыль - для поиска ресурса по названию при выборе в дропдауне

    public void UpdatePanel(Resource resource)
    {
        resourceImage.sprite = resource.Image;
        resourceText.text = resource.Name;
        dropdown.value = GameManager.FindCurrentResourceNumberByName(resource.Name);
    }

    //  Поднятие и снятие флажков при взаимодействии пользователя с панелью
    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayerController.Instance.isMouseOnUI = true;
        Debug.Log("mouse on panel");
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        PlayerController.Instance.isMouseOnUI = false;
        Debug.Log("mouse out of panel");
    }
}
