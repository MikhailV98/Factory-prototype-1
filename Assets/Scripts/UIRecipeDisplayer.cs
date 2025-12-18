using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRecipeDisplayer : MonoBehaviour
{
    [SerializeField] RectTransform inResourcesParent;
    [SerializeField] RectTransform outResourcesParent;
    [SerializeField] int maxResourcesInRow = 3;
    [SerializeField] GameObject resourceImageTemplate;
    public bool isShowNumbers = true;

    Recipe currentRecipe;

    //  Установка текущего отображаемого рецепта
    public void SetCurrentRecipe(Recipe recipe)
    {
        if (recipe != null)
        {
            currentRecipe = recipe;
        }
    }

    //  Метод обновления дисплеера
    public void Visualize()
    {
        ClearRecipeDisplay();
        if (currentRecipe != null)
        {
            if (inResourcesParent != null)
                VisualizeResources(currentRecipe.InResources, inResourcesParent);
            VisualizeResources(currentRecipe.OutResource, outResourcesParent);
        }
    }

    //  2 перегрузки метода отображения - для списков ресурсов и для единичных
    void VisualizeResources(List<Recipe.RecipeResource> resourcesList,RectTransform parentTransform)
    {
        List<RectTransform> panelsList;
        //  Create panels with HorizontalLayout
        if (resourcesList.Count <= 2)
        {
            //  Creating 1 panel
            panelsList = CreatePanels(1, parentTransform);

        }
        else if (resourcesList.Count <= maxResourcesInRow * 2)
        {
            //  Creating 2 panels
            panelsList = CreatePanels(2, parentTransform);
        }
        else
        {
            //  Creating minimum panels
            panelsList = CreatePanels(Mathf.CeilToInt((float)resourcesList.Count/(float)maxResourcesInRow), parentTransform);
        }

        //  Loading panels with images
        for(int i = 0; i < resourcesList.Count; i++)
        {
            int numberInList = Mathf.FloorToInt((float)i / (float)maxResourcesInRow);
            InstantiateImage(resourcesList[i], panelsList[numberInList]);
        }
    }
    void VisualizeResources(Recipe.RecipeResource resource, RectTransform parentTransform)
    {
        RectTransform panel = CreatePanels(1, parentTransform)[0];

        InstantiateImage(resource, panel);
    }

    //  Метод создания панелей с компонентом горизонтальной разметки
    public static List<RectTransform> CreatePanels(int count, RectTransform parentTransform)
    {

        List<RectTransform> panelsTransformList = new List<RectTransform>();
        for (int i = 0; i < count; i++)
        {
            GameObject newPanel = new GameObject("HorizontalLayout");
            newPanel.transform.parent = parentTransform;
            newPanel.transform.localScale = Vector3.one;
            newPanel.transform.localPosition = Vector3.zero;
            newPanel.transform.localRotation = Quaternion.Euler(Vector3.zero);

            newPanel.AddComponent<CanvasRenderer>();
            HorizontalLayoutGroup horizontalLayoutGroup = newPanel.AddComponent<HorizontalLayoutGroup>();
            horizontalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            horizontalLayoutGroup.childControlWidth = false;
            horizontalLayoutGroup.childControlHeight = false;

            panelsTransformList.Add((RectTransform)newPanel.transform);
        }
        return panelsTransformList;
    }

    //  Метод создаёт изображения в зависимости от настройки дисплеера - добавляет либо статичные картинки, либо картинки с подписью
    void InstantiateImage(Recipe.RecipeResource resource, RectTransform parent)
    {
        if (isShowNumbers)
        {
            GameObject newResourceImage = Instantiate(resourceImageTemplate, parent);
            newResourceImage.GetComponent<UIResourceImage>().Init(resource);
        }
        else
        {
            GameObject newResourceImageGO = new GameObject();
            newResourceImageGO.AddComponent<CanvasRenderer>();
            Image newImageImage = newResourceImageGO.AddComponent<Image>();
            newImageImage.sprite = resource.resource.Image;
            newImageImage.preserveAspect = true;

            RectTransform newResourceImageTransform = newResourceImageGO.transform as RectTransform;

            newResourceImageTransform.SetParent(parent);
            newResourceImageTransform.localPosition = Vector3.zero;
            newResourceImageTransform.localRotation = Quaternion.Euler(Vector3.zero);
            newResourceImageTransform.localScale = Vector3.one*1.5f;
            
        }
    }
    
    //  Очистка дисплеера
    void ClearRecipeDisplay()
    {
        if (inResourcesParent != null)
            foreach (Transform child in inResourcesParent)
            {
                GameObject.Destroy(child.gameObject);
            }
        foreach (Transform child in outResourcesParent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
