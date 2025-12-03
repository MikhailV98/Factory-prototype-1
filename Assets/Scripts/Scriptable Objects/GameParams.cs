using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Params Config", menuName = "Configs/Game Params Config", order = 1)]
public class GameParams : ScriptableObject
{
    //  Конфиг основных параметров
    public Material errorMaterial;      //  Материал при невозможности строительства
    public Material selectionMaterial;  //  Материал при выделении
    public Resource emptyResource;
    public List<Resource> resourcesList;//  Список используемых ресурсов
    public List<Recipe> creatorRecipes;
    public List<Recipe> connectorRecipes;
}
