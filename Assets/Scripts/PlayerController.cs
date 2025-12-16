using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum PlayerState
{
    Loading,
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
    public SellerBuildingPanel sellerBuildingPanelUI;
    public CreatorBuildingUI creatorBuildingUI;

    public Building selectedBuilding;

    public bool isMouseOnUI = false;

    public UnityEvent onBuildingCreated = new UnityEvent();
    public UnityEvent onBuildingDestroyed = new UnityEvent();
    public UnityEvent onBuildingUpdate = new UnityEvent();
    List<Building> buildingsList = new List<Building>();
    [SerializeField] AudioClip buildingSelectionSound;
      

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        if (SaveSystem.currentPlayerProfile.buildingsList != null)
        {
            LoadBuildingsFromProfile();
        }
        else
            currentState = PlayerState.Selecting;

        onBuildingUpdate.AddListener(SaveBuildingsToProfile);
    }
    void LoadBuildingsFromProfile()
    {
        currentState = PlayerState.Loading;
        foreach (SaveSystem.PlayerProfile.BuildingInfo building in SaveSystem.currentPlayerProfile.buildingsList)
        {
            switch (building.buildingType)
            {
                case BuildingTypes.Creator:
                    {

                        CreatorBuilding newBuilding = CreateBuildingOnPoint(building.buildingType, building.buildingPosition) as CreatorBuilding;
                        newBuilding.ChangeRecipe(building.buildingRecipe);
                        newBuilding.isWorking = building.isWorking;
                        break;
                    }
                case BuildingTypes.Connector:
                    {
                        CreatorBuilding newBuilding = CreateBuildingOnPoint(building.buildingType, building.buildingPosition) as CreatorBuilding;
                        newBuilding.ChangeRecipe(building.buildingRecipe);
                        newBuilding.isWorking = building.isWorking;
                        break;
                    }
                case BuildingTypes.Seller:
                    {
                        SellerBuilding newBuilding = CreateBuildingOnPoint(building.buildingType, building.buildingPosition) as SellerBuilding;
                        newBuilding.ChangeResource(building.buildingResource);
                        newBuilding.isWorking = building.isWorking;
                        break;
                    }
                default:
                    {
                        Debug.LogError("Trying to load unexpected building");
                        break;
                    }
            }
        }

        currentState = PlayerState.Selecting;
    }
    void SaveBuildingsToProfile()
    {
        List<SaveSystem.PlayerProfile.BuildingInfo> buildingInfoList = new List<SaveSystem.PlayerProfile.BuildingInfo>();
        foreach (Building building in buildingsList)
            buildingInfoList.Add(building.ToBuildingInfo());

        SaveSystem.currentPlayerProfile.buildingsList = buildingInfoList;
    }

    Building CreateBuildingOnPoint(BuildingTypes buildingType, Vector3 buildingPosition)
    {
        PrepareToBuild(GameManager.Instance.gameParams.GetBuildingsDictionary()[buildingType]);
        Building building = currentBuilding.GetComponent<Building>();
        MoveBuildingToPoint(buildingPosition);
        BuildObject();

        return building;
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
                SelectBuilding(clickedBuilding);
            }
        }
        else
        {
            if (!isMouseOnUI)
                DeselectBuilding();
        }
    }
    void SelectBuilding(Building building)
    {
        
        selectedBuilding = building;
        selectedBuilding.OnSelect();
        GameManager.Instance.PlaySound(buildingSelectionSound);
    }
    void DeselectBuilding()
    {

        if (selectedBuilding != null)
        {
            selectedBuilding.OnDeselect();
            selectedBuilding = null;
        }
    }
    //  Отмена постройки при нажатии ПКМ
    public void MouseInputSecondActionHandler()
    {
        if (currentState == PlayerState.Building)
        {
            GameManager.Instance.ReturnCost(currentBuilding.GetComponent<Building>());
            Destroy(currentBuilding);
            ReleaseBuilding();
        }
    }
    void BuildObject()
    {
        if (currentBuildingHelper.CheckCollisions())
        {
            currentBuilding.transform.SetParent(buildingsContainer);
            Building newBuilding = currentBuilding.GetComponent<Building>();
            newBuilding.enabled = true;
            currentBuilding.GetComponent<BuildingHUD>().enabled = true;
            newBuilding.OnPlaced();
            ReleaseBuilding();

            onBuildingCreated.Invoke();
            
            if (!buildingsList.Contains(newBuilding))
                buildingsList.Add(newBuilding);

            if (currentState != PlayerState.Loading)
            {
                SelectBuilding(newBuilding);
                SaveBuildingsToProfile();
            }
        }
    }
    void ReleaseBuilding()
    {
        currentBuildingHelper.RemoveComponent();
        currentBuildingHelper = null;
        currentBuilding = null;
        if (currentState != PlayerState.Loading)
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
    public Vector3 GetMousePositionInScene()
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
    //  Для префабов
    public void PrepareToBuild(GameObject building)
    {
        if (currentState != PlayerState.Building)
        {
            if (currentState != PlayerState.Loading)
                currentState = PlayerState.Building;
            currentBuilding = Instantiate(building);
            currentBuildingHelper = currentBuilding.GetComponent<BuildingHelper>();
        }
    }
    //  Для готовых зданий
    public void PrepareToBuild(Building building)
    {
        if (currentState != PlayerState.Building)
        {
            currentState = PlayerState.Building;
            currentBuilding = building.gameObject;
            currentBuildingHelper = currentBuilding.GetComponent<BuildingHelper>();
        }
    }

    public void ChangeRecipeOnSelectedBuilding(int numberInRecipesList)
    {
        CreatorBuilding building;
        if(building = selectedBuilding as CreatorBuilding)
        {
            Recipe newRecipe = building.currentRecipe;
            switch (building.buildingType)
            {
                case BuildingTypes.Creator:
                    {
                        newRecipe = GameManager.Instance.gameParams.creatorRecipes[numberInRecipesList];
                        break;
                    }
                case BuildingTypes.Connector:
                    {
                        newRecipe = GameManager.Instance.gameParams.connectorRecipes[numberInRecipesList];
                        break;
                    }
            }
            building.ChangeRecipe(newRecipe);
        }
    }
    public void ChangeResourceOnSelectedBuilding(Resource resource)
    {
        SellerBuilding building;
        if (building = selectedBuilding as SellerBuilding)
            building.ChangeResource(resource);
    }

    public void DeleteSelectedBuilding()
    {
        StartCoroutine(DeletingCurrentBuilding());
    }
    IEnumerator DeletingCurrentBuilding()
    {
        selectedBuilding.DeleteBuilding();
        yield return new WaitForNextFrameUnit();
        onBuildingDestroyed.Invoke();
    }
    public void OpenMenu(RectTransform menu) => menu.gameObject.SetActive(!menu.gameObject.activeInHierarchy);
    public void ExitToMainMenu()
    {
        SaveSystem.SaveProfile();
        SceneManager.LoadScene(0);
    }

}

