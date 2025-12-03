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

    Recipe currentRecipe;

    public void SetCurrentRecipe(Recipe recipe)
    {
        if (recipe != null)
        {
            currentRecipe = recipe;
        }
    }

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
            GameObject newResourceImage = Instantiate(resourceImageTemplate, panelsList[numberInList]);
            newResourceImage.GetComponent<UIResourceImage>().Init(resourcesList[i]);
        }
    }
    void VisualizeResources(Recipe.RecipeResource resource, RectTransform parentTransform)
    {
        Transform panel = CreatePanels(1, parentTransform)[0];

        GameObject newResourceImage = Instantiate(resourceImageTemplate, panel);
        newResourceImage.GetComponent<UIResourceImage>().Init(resource);
    }
    List<RectTransform> CreatePanels(int count, RectTransform parentTransform)
    {

        List<RectTransform> panelsTransformList = new List<RectTransform>();
        for(int i = 0; i < count; i++)
        {
            GameObject newPanel = new GameObject("HorizontalLayout");
            newPanel.transform.parent = parentTransform;
            newPanel.AddComponent<CanvasRenderer>();
            HorizontalLayoutGroup horizontalLayoutGroup =  newPanel.AddComponent<HorizontalLayoutGroup>();
            horizontalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            horizontalLayoutGroup.childControlWidth = false;
            horizontalLayoutGroup.childControlHeight = false;

            panelsTransformList.Add((RectTransform)newPanel.transform);
        }
        return panelsTransformList;
    }

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
