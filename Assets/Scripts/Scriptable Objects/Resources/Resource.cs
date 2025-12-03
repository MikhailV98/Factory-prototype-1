using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Recource",menuName ="Objects/Resource",order =1)]
public class Resource : ScriptableObject
{
    //  Базовый класс объектов-ресурсов
    public bool CanBeStored;
    public Sprite Image;    //  Исконка
    public string Name;     //  Название
    public int Cost;        //  Стоимость в монетах
}
