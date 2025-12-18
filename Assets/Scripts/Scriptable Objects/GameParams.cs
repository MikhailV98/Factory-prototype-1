using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Params Config", menuName = "Configs/Game Params Config", order = 1)]
public class GameParams : ScriptableObject
{
    //  Конфиг основных параметров
    public Material errorMaterial;      //  Материал при невозможности строительства
    public Material selectionMaterial;  //  Материал при выделении
    public Resource emptyResource;  //  Пустой ресурс
    public Sprite moneySprite;  //  Спрайт денег
    public List<Resource> resourcesList;//  Список используемых ресурсов
    
    //  Списки рецептов для зданий
    public List<Recipe> creatorRecipes;
    public List<Recipe> connectorRecipes;

    public List<BuildingObject> buildingsList;

    //  Звуки
    public AudioClip openDropdownSound;
    public AudioClip closeDropdownSound;

    //  Словарь для связи типа здания и префаба
    public Dictionary<BuildingTypes,GameObject> GetBuildingsDictionary()
    {
        Dictionary<BuildingTypes, GameObject> dictionary = new Dictionary<BuildingTypes, GameObject>();
        foreach (BuildingObject buildingObj in buildingsList)
            dictionary.Add(buildingObj.Prefab.GetComponent<Building>().buildingType, buildingObj.Prefab);
        return dictionary;
    }
}
