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
    public void Init(Recipe.RecipeResource resource)
    {
        image.sprite = resource.resource.Image;
        text.text = resource.count.ToString();
    }
}
