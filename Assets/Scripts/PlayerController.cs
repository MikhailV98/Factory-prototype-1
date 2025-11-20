using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum PlayerState
{
    Building,
    Selecting,
    ChoosingResourceInPanel
}

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public PlayerState currentState;
    [SerializeField] GameObject currentBuildingPrefab;
    public Transform buildingsContainer;
    GameObject currentBuilding;
    BuildingHelper currentBuildingHelper;
    public ProductBuildingPanel productBuildingPanelUI;
    public SellerBuildingPanel sellerBuildingPanelUI;

    public Building selectedBuilding;

    public bool isMouseOnUI = false;


    //public GameParams gp;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        currentState = PlayerState.Selecting;

    }

    // Update is called once per frame
    void Update()
    {

        //  Постоянная обработка
        if (currentState == PlayerState.Building)
        {
            //  Move building prefab
            MoveBuildingToPoint(GetMousePositionInScene());
        }

        //  Обработка ЛКМ
        if (Input.GetMouseButtonDown(0))
        {
            MouseInputActionHandler();
        }
        //  Обработка ПКМ
        if (Input.GetMouseButtonDown(1))
        {
            MouseInputSecondActionHandler();
        }
    }
    public void MouseInputActionHandler()
    {
        switch (currentState)
        {
            case PlayerState.Selecting:
                {
                    SelectionClickHandler();
                    break;
                }
            case PlayerState.Building:
                {
                    //  Check if can build
                    BuildObject();
                    break;
                }
            default:
                {
                    Debug.LogError("Unknown player state");
                    break;
                }
        }
    }
    void SelectionClickHandler()
    {
        Building clickedBuilding;
        if ((clickedBuilding = CheckBuildingClick()) != null)
        {
            if (clickedBuilding != selectedBuilding)
            {
                DeselectBuilding();
                selectedBuilding = clickedBuilding;
                selectedBuilding.OnSelect();
            }
        }
        else
        {
            if (!isMouseOnUI)
                DeselectBuilding();
        }
    }
    void DeselectBuilding()
    {

        if (selectedBuilding != null)
        {
            selectedBuilding.OnDeselect();
            selectedBuilding = null;
        }
    }
    public void MouseInputSecondActionHandler()
    {
        if (currentState == PlayerState.Building)
        {
            Destroy(currentBuilding);
            ReleaseBuilding();
        }
    }
    void BuildObject()
    {
        if (currentBuildingHelper.CheckCollisions())
        {
            currentBuilding.transform.SetParent(buildingsContainer);
            currentBuilding.GetComponent<Building>().enabled = true;
            currentBuilding.GetComponent<BuildingUI>().enabled = true;
            ReleaseBuilding();
        }
    }
    void ReleaseBuilding()
    {
        currentBuildingHelper.RemoveComponent();
        currentBuildingHelper = null;
        currentBuilding = null;
        currentState = PlayerState.Selecting;
    }

    Building CheckBuildingClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit))
        {
            return raycastHit.transform.gameObject.GetComponent<Building>();
        }
        else
        {
            return null;
        }
    }
    Vector3 GetMousePositionInScene()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit))
        {
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }
    void MoveBuildingToPoint(Vector3 point)
    {
        Vector3 convertedPoint = new Vector3((int)point.x, 0, (int)point.z);
        currentBuilding.transform.position = convertedPoint;
        if (!currentBuildingHelper.CheckCollisions())
        {
            GameManager gm = GameManager.Instance;
            GameParams gp = gm.gameParams;

            Material m = gp.errorMaterial;
            currentBuildingHelper.ChangeMaterial(m);
        }
    }

    public void PrepareToBuild(GameObject building)
    {
        if (currentState != PlayerState.Building)
        {
            currentState = PlayerState.Building;
            currentBuilding = Instantiate(building);
            currentBuildingHelper = currentBuilding.GetComponent<BuildingHelper>();
        }
    }

    public void ChangeResourceOnSelectedBuilding(Resource r) => selectedBuilding.ChangeResource(r);

    public void DeleteSelectedBuilding() => selectedBuilding.DeleteBuilding();
}

