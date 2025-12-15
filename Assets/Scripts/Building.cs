using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    //  Базовый класс всех зданий
    protected MeshRenderer meshRenderer;

    [SerializeField] Material defaultMaterial;  //  Необходим для возвращения зданию цвета после снятия выделения и при строительстве
    public BuildingTypes buildingType;

    public virtual SaveSystem.PlayerProfile.BuildingInfo ToBuildingInfo()
    {
        SaveSystem.PlayerProfile.BuildingInfo buildingInfo = new SaveSystem.PlayerProfile.BuildingInfo();
        buildingInfo.buildingPosition = transform.position;
        buildingInfo.buildingType = buildingType;
        return buildingInfo;
    }

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    //  Методы выделения и снятия выделения с объектов - из общего только смена материала.
    //  Дочерние здания (как продуктовый, мб усиляющий) - переопределяют методы в соответствии со своими нуждами
    public virtual void OnSelect()  {meshRenderer.material = GameManager.Instance.gameParams.selectionMaterial;Debug.Log("building selected"); }
    public virtual void OnDeselect()
    { 
        meshRenderer.material = defaultMaterial;
        PlayerController.Instance.onBuildingUpdate.Invoke();
    }

    public virtual void DeleteBuilding()
    {
        Destroy(this.gameObject);
    }
    public virtual void MoveBuilding()
    {
        OnDeselect();
        this.gameObject.AddComponent<BuildingHelper>();
        PlayerController.Instance.PrepareToBuild(this);
        GetComponent<BuildingHUD>().enabled = false;
        this.enabled = false;
    }
    public virtual void OnPlaced() { }
}
