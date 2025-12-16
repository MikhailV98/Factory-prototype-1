using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellerBuildingUI : BuildingHUD
{
    //  UI вокруг продающего здания

    [SerializeField] Image resourceImage;           //  Иконка ресурса на здании
    [SerializeField] Slider resourceProgressBar;    //  Прогрессбар ресурса на передней части
    [SerializeField] UIResourceParticleEffect resourceParticleEffect;

    public void UpdateUI(Resource resource)
    {
        resourceImage.sprite = resource.Image;
        resourceProgressBar.value = 0;
    }
    public void UpdateSlider(float value)
    {
        resourceProgressBar.value = value;
    }
    public void SpawnResourceEffect()
    {
        resourceParticleEffect.SpawnEffect(GameManager.Instance.gameParams.moneySprite);
    }
}
