using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    //  Базовый класс всех зданий
    protected MeshRenderer meshRenderer;

    [SerializeField] Material defaultMaterial;  //  Необходим для возвращения зданию цвета после снятия выделения и при строительстве

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    //  Методы выделения и снятия выделения с объектов - из общего только смена материала.
    //  Дочерние здания (как продуктовый, мб усиляющий) - переопределяют методы в соответствии со своими нуждами
    public virtual void OnSelect() => meshRenderer.material = GameManager.Instance.gameParams.selectionMaterial;
    public virtual void OnDeselect() => meshRenderer.material = defaultMaterial;

    public virtual void ChangeResource(Resource newResource) { }

    public virtual void DeleteBuilding()
    {
        Destroy(this.gameObject);
    }
}
