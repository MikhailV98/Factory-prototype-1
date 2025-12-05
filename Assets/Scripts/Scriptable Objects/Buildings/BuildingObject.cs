using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Objects/Building", order = 3)]
public class BuildingObject : ScriptableObject
{
    public string Name;
    public GameObject Prefab;
    public int costBase;
    public float costIncrement;
}
