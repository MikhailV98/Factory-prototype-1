using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameMath
{
    public static int GetBuildingCost(BuildingObject buildingObject, int currentBuildingsOfTypeCount)
    {
        if (GameManager.Instance)
        {
            return buildingObject.costBase + Mathf.RoundToInt(buildingObject.costIncrement * currentBuildingsOfTypeCount);
        }
        else
        {
            Debug.LogError("Can't find GameManager");
            return Int32.MaxValue;
        }
    }
}
