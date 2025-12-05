using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBuildingButton : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public BuildingObject buildingObject;
    BuildingTypes buildingType;

    public void Init(BuildingObject buildingObject)
    {
        this.buildingObject = buildingObject;
        nameText.text = buildingObject.Name;
        GameObject buildingPrefab = buildingObject.Prefab;
        buildingType = buildingPrefab.GetComponent<Building>().buildingType;
        GetComponent<Button>().onClick.AddListener(TryBuild);
        UpdateCost();
    }

    void TryBuild()
    {
        if (GameManager.Instance.TryBuild(buildingType))
            PlayerController.Instance.PrepareToBuild(buildingObject.Prefab);
        else 
            Debug.Log("Not enought money");
    }

    public void UpdateCost()
        => costText.text = GameMath.GetBuildingCost(buildingObject,GameManager.Instance.GetBuildingsCount(buildingType)).ToString();
}
