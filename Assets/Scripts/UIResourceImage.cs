using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIResourceImage : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI text;

    public void Init(Sprite newSprite, string newText)
    {
        image.sprite = newSprite;
        text.text = newText;
    }
    public void Init(Recipe.RecipeResource recipeResource)
    {
        if (recipeResource != null && recipeResource.resource != null)
        {
            image.sprite = recipeResource.resource.Image;
            text.text = recipeResource.count.ToString();
        }
        else InitWithNullResource();
    }
    public void Init(Quest.QuestItem questItem)
    {
        if (questItem != null && questItem.resource != null)
        {
            image.sprite = questItem.resource.Image;
            text.text = questItem.count.ToString();
        }
        else InitWithNullResource();
    }
    void InitWithNullResource()
    {
        image.sprite = GameManager.Instance.gameParams.emptyResource.Image;
        text.text = "0";
    }

    public void ChangeText(string newText) => text.text = newText;
}
