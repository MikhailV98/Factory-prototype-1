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
    [SerializeField] UIResourceDropdown dropdown;
    [SerializeField] TextMeshProUGUI pauseButtonText;
    [SerializeField] Slider progressionSlider;
    [SerializeField] TextMeshProUGUI moneyText;
    SellerBuilding currentBuilding;

    public void UpdatePanel(Resource resource)
    {
        resourceImage.sprite = resource.Image;
        resourceText.text = resource.Name;
        moneyText.text = resource.Cost.ToString();
        dropdown.value = GameManager.FindCurrentResourceNumberByName(resource.Name);
        UpdatePauseButton();
    }
    public void SetCurrentBuilding(SellerBuilding newBuilding)
        => currentBuilding = newBuilding;
    public SellerBuilding GetCurrentBuilding()
        => currentBuilding;
    public void UpdateProgressionSlider(float value)
        => progressionSlider.value = value;
    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayerController.Instance.isMouseOnUI = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        PlayerController.Instance.isMouseOnUI = false;
    }
    void UpdatePauseButton()
    {
        if (currentBuilding.isWorking)
            pauseButtonText.text = "Pause";
        else pauseButtonText.text = "Continue";
    }
    public void SwitchStateOnCurrentBuilding()
    {
        currentBuilding.SwitchSellingState();
        UpdatePauseButton();
    }
    public void MoveBuildingClick()
    {
        currentBuilding.MoveBuilding();
        PlayerController.Instance.isMouseOnUI = false;
    }
}
