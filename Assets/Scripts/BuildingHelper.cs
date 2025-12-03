using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHelper : MonoBehaviour
{
    //  Скрипт для помощи в строительстве
    //  Определяет возможность постройки в данном месте и при невоможности подсвечивает
    bool isOutOfCollisions = true;
    Material baseMat;

    private void Start()
    {
        baseMat = GetComponent<MeshRenderer>().material;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Building"))
        {
            isOutOfCollisions = false;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Building"))
        {
            isOutOfCollisions = true;
            ReturnDefaultMaterial();
        }
    }

    public bool CheckCollisions()
    {
        return isOutOfCollisions;
    }
    public void ChangeMaterial(Material m)
    {
        GetComponent<MeshRenderer>().material = m;
    }
    public void ReturnDefaultMaterial()
    {
        GetComponent<MeshRenderer>().material = baseMat;
    }

    //  При окончании строительства выпиливаем
    public void RemoveComponent()
    {
        Destroy(this);
    }
}
