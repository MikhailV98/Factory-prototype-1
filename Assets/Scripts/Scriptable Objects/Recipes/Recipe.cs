using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Objects/Recipe", order = 2)]
public class Recipe : ScriptableObject
{
    //  Базовый класс для рецептов
    public bool CanBeProduced;
    public RecipeType recipeType;
    public List<RecipeResource> InResources;
    public RecipeResource OutResource;
    public float RecipeHardness;

    [Serializable]
    public class RecipeResource
    {
        public Resource resource;
        public int count;
    }
}

public enum RecipeType
{
    ZeroToOne,
    MultipleToOne,
    MultipleToMultiple
}
