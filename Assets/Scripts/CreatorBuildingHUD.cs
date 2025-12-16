using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatorBuildingHUD : BuildingHUD
{

    //  Класс для UI вокруг производящего здания
    [SerializeField] Image resourceImage;           //  Иконка ресурса на здании
    [SerializeField] Slider resourceProgressBar;    //  Прогрессбар ресурса на передней части
    [SerializeField] UIResourceParticleEffect resourceParticleEffect;
    [SerializeField] RectTransform topHUDTransform;
    [SerializeField] GameObject creatorRecipeDisplayerPrefab;
    [SerializeField] GameObject connectorRecipeDisplayerPrefab;
    UIRecipeDisplayer recipeDisplayer;

    public void Init(BuildingTypes buildingType)
    {
        switch (buildingType)
        {
            case BuildingTypes.Creator:
                {
                    recipeDisplayer = Instantiate(creatorRecipeDisplayerPrefab, topHUDTransform).GetComponent<UIRecipeDisplayer>();
                    break;
                }
            case BuildingTypes.Connector:
                {
                    recipeDisplayer = Instantiate(connectorRecipeDisplayerPrefab, topHUDTransform).GetComponent<UIRecipeDisplayer>();
                    break;
                }
            default:
                {
                    recipeDisplayer = null;
                    break;
                }
        }
        recipeDisplayer.isShowNumbers = false;
        RectTransform displayerTransform = (RectTransform)recipeDisplayer.transform;
        displayerTransform.anchorMin = Vector2.zero;
        displayerTransform.anchorMin = Vector2.zero;
        displayerTransform.offsetMin = Vector2.zero;
        displayerTransform.offsetMax= Vector2.zero;

    }

    public void UpdateUI(Recipe recipe)
    {
        if (recipe != null)
        {
            resourceImage.sprite = recipe.OutResource.resource.Image;
            resourceProgressBar.value = 0;
            recipeDisplayer.SetCurrentRecipe(recipe);
            recipeDisplayer.Visualize();
        }
        else resourceImage.sprite = GameManager.Instance.gameParams.emptyResource.Image;
    }
    public void UpdateSlider(float value)
    {
        resourceProgressBar.value = value;
    }
    public void SpawnResourceEffect()
    {
        resourceParticleEffect.SpawnEffect(resourceImage.sprite);
    }
}
