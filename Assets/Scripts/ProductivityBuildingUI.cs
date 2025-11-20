using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductivityBuildingUI : BuildingUI
{
    //  Класс для UI вокруг производящего здания
    [SerializeField] Image resourceImage;           //  Иконка ресурса на здании
    [SerializeField] Slider resourceProgressBar;    //  Прогрессбар ресурса на передней части

    public void UpdateUI(Resource resource)
    {
        resourceImage.sprite = resource.Image;
        resourceProgressBar.value = 0;
    }
    public void UpdateSlider(float value)
    {
        resourceProgressBar.value = value;
    }
}
