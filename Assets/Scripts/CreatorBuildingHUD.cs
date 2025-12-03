using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatorBuildingHUD : BuildingHUD
{
    //  УСТАРЕВШИЙ ДЛЯ РЕСУРСОВ, ОБНОВИТЬ ДЛЯ РЕЦЕПТОВ

    //  Класс для UI вокруг производящего здания
    [SerializeField] Image resourceImage;           //  Иконка ресурса на здании
    [SerializeField] Slider resourceProgressBar;    //  Прогрессбар ресурса на передней части

    public void UpdateUI(Recipe recipe)
    {
        if (recipe != null)
        {
            resourceImage.sprite = recipe.OutResource.resource.Image;
            resourceProgressBar.value = 0;
        }
        else resourceImage.sprite = GameManager.Instance.gameParams.emptyResource.Image;
    }
    public void UpdateSlider(float value)
    {
        resourceProgressBar.value = value;
    }
}
