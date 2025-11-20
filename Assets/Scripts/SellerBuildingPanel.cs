using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SellerBuildingPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //  Скрипт взаимодействия с панелью производящего здания
    [SerializeField] Image resourceImage;
    [SerializeField] TextMeshProUGUI resourceText;
    [SerializeField] ProductBuildingDropDown dropdown;

    [SerializeField] TextMeshProUGUI moneyText;

    //  Костыль - для поиска ресурса по названию при выборе в дропдауне

    public void UpdatePanel(Resource resource)
    {
        resourceImage.sprite = resource.Image;
        resourceText.text = resource.Name;
        moneyText.text = resource.Cost.ToString();
        dropdown.value = GameManager.FindCurrentResourceNumberByName(resource.Name);
    }

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
