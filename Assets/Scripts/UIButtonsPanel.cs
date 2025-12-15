using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonsPanel : MonoBehaviour
{
    [SerializeField] GameObject buttonPrefab;
    List<UIBuildingButton> buttonsList;

    private void Start()
    {
        buttonsList = new List<UIBuildingButton>();
        PlayerController.Instance.onBuildingCreated.AddListener(UpdatePanel);
        PlayerController.Instance.onBuildingDestroyed.AddListener(UpdatePanel);
        GenerateButtonsPanel();
    }

    void GenerateButtonsPanel()
    {
        //  Creating Buttons
        foreach(BuildingObject buildingObject in GameManager.Instance.gameParams.buildingsList)
        {
            UIBuildingButton newButton = Instantiate(buttonPrefab, transform).GetComponent<UIBuildingButton>();
            newButton.Init(buildingObject);
            buttonsList.Add(newButton);
        }
    }

    void UpdatePanel()
    {
        //  Update Buttons Values
        foreach (UIBuildingButton button in buttonsList)
            button.UpdateCost();
    }
}
