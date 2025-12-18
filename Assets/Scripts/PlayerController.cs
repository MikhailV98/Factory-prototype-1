using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//  Перечень возможных состояний пользователя
public enum PlayerState
{
    Loading,
    Building,
    Selecting,
    ChoosingResourceInPanel
}

public class PlayerController : MonoBehaviour
{
    //  Управление вводом пользователя в геймплее

    public static PlayerController Instance;    //  Синглтон для доступа к настройкам

    public PlayerState currentState;    //  Текущее состояние пользователя
    
    BuildingHelper currentBuildingHelper;   //  BuildingHelper здания, которое строится или переносится

    public Transform buildingsContainer;    //  Transform для зданий
    GameObject currentBuilding;
    
    //  Окна зданий
    public SellerBuildingPanel sellerBuildingPanelUI;
    public CreatorBuildingUI creatorBuildingUI;

    public Building selectedBuilding;   //  Текущее выделенное здание

    public bool isMouseOnUI = false;    //  Находится ли курсор в зоне UI

    //  Ивенты чтобы что-то делать
    public UnityEvent onBuildingCreated = new UnityEvent();
    public UnityEvent onBuildingDestroyed = new UnityEvent();
    public UnityEvent onBuildingUpdate = new UnityEvent();

    List<Building> buildingsList = new List<Building>();    //  Список всех зданий
    
    [SerializeField] AudioClip buildingSelectionSound;      //  Звук для выбора зданий
      
    /*-----Подготовка объекта-----*/
    void Awake()
    {
        //  Реализация Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    private void Start()
    {
        //  Если существует список зданий у данного профиля, загружаем его
        if (SaveSystem.currentPlayerProfile.buildingsList != null)
            LoadBuildingsFromProfile();
        else
            currentState = PlayerState.Selecting;

        //  Сохраняем список когда пользователь что-либо делает со зданиями
        onBuildingUpdate.AddListener(SaveBuildingsToProfile);
    }

    /*-----Работа с сохранениями-----*/
    //  Загрузка зданий из профиля
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
    //  Сохранение зданий в профайл
    void SaveBuildingsToProfile()
    {
        List<SaveSystem.PlayerProfile.BuildingInfo> buildingInfoList = new List<SaveSystem.PlayerProfile.BuildingInfo>();
        foreach (Building building in buildingsList)
            buildingInfoList.Add(building.ToBuildingInfo());

        SaveSystem.currentPlayerProfile.buildingsList = buildingInfoList;
    }
    
    /*-----Обработка ввода пользователя-----*/
    void Update()
    {

        //  Постоянная обработка перемещения мыши когда строим здание
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

    //  Обработка ЛКМ
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
    //  Обработка выбора во время состояния "выделение"
    void SelectionClickHandler()
    {
        Building clickedBuilding;
        if ((clickedBuilding = CheckBuildingClick()) != null)
        {
            //  При клике на здание проверяем, соответствует ли оно выбранному и отменяем выбор, если нет
            if (clickedBuilding != selectedBuilding)
            {
                DeselectBuilding();
                SelectBuilding(clickedBuilding);
            }
        }
        else
        {
            //  При клике вне здания проверяем, находится ли мышь в UI и отменяем выбор, если нет
            if (!isMouseOnUI)
                DeselectBuilding();
        }
    }
    //  Обработка ПКМ
    public void MouseInputSecondActionHandler()
    {
        //  ПКМ позволяет отменить постройку здания
        if (currentState == PlayerState.Building)
        {
            GameManager.Instance.ReturnCost(currentBuilding.GetComponent<Building>());
            Destroy(currentBuilding);
            ReleaseBuilding();
        }
    }

    //  Проверка, попадает ли курсор на здание
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
    //  Положение мыши в сцене
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

    /*-----Взаимодействие со зданиями-----*/
    //  Выбор здания
    void SelectBuilding(Building building)
    {
        
        selectedBuilding = building;
        selectedBuilding.OnSelect();
        GameManager.Instance.PlaySound(buildingSelectionSound);
    }
    //  Отмена выбора здания
    void DeselectBuilding()
    {

        if (selectedBuilding != null)
        {
            selectedBuilding.OnDeselect();
            selectedBuilding = null;
        }
    }
    
    //  Разные методы для смены задания зданий
    public void ChangeRecipeOnSelectedBuilding(int numberInRecipesList)
    {
        //  Выбор рецепта происходит по номеру в списке
        CreatorBuilding building;
        if (building = selectedBuilding as CreatorBuilding)
        {
            Recipe newRecipe = building.currentRecipe;
            //  В зависимости от типа здания смотрим тот или иной список
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

    //  Снос здания
    public void DeleteSelectedBuilding()
        => StartCoroutine(DeletingCurrentBuilding());
    IEnumerator DeletingCurrentBuilding()
    {
        selectedBuilding.DeleteBuilding();
        //  Ждём завершения процессов, чтобы опопвестить о сносе здания
        yield return new WaitForNextFrameUnit();
        onBuildingDestroyed.Invoke();
    }

    /*-----Постройка зданий-----*/
    /* Две версии метода для подготовки к постройке или переносу здания соответственно */
    //  Для префабов
    public void PrepareToBuild(GameObject building)
    {
        if (currentState != PlayerState.Building)
        {
            //  Если мы загружаем здания из профиля, нам не нужно менять состояние игрока
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

    //  Метод для постройки здания
    void BuildObject()
    {
        if (currentBuildingHelper.CheckCollisions())
        {
            currentBuilding.transform.SetParent(buildingsContainer);    //  Помещаем здание в контейнер зданий
            
            //  Включаем компоненты здания и интерфейса здания
            Building newBuilding = currentBuilding.GetComponent<Building>();
            newBuilding.enabled = true;
            currentBuilding.GetComponent<BuildingHUD>().enabled = true;
            newBuilding.OnPlaced(); //  Особые действия здания при размещении

            //  Отпускаем здание
            ReleaseBuilding();

            onBuildingCreated.Invoke();
            
            //  Добавляем здание в список
            if (!buildingsList.Contains(newBuilding))
                buildingsList.Add(newBuilding);

            //  Если данный метод вызывается не на загрузке зданий из списка, выделяем его и сохраняем в профиль
            if (currentState != PlayerState.Loading)
            {
                SelectBuilding(newBuilding);
                SaveBuildingsToProfile();
            }
        }
    }
    //  Спавн определённого здания сразу на точке
    Building CreateBuildingOnPoint(BuildingTypes buildingType, Vector3 buildingPosition)
    {
        PrepareToBuild(GameManager.Instance.gameParams.GetBuildingsDictionary()[buildingType]);
        Building building = currentBuilding.GetComponent<Building>();
        MoveBuildingToPoint(buildingPosition);
        BuildObject();

        return building;
    }
    //  Отпускание здания из управления
    void ReleaseBuilding()
    {
        currentBuildingHelper.RemoveComponent();
        currentBuildingHelper = null;
        currentBuilding = null;
        if (currentState != PlayerState.Loading)
            currentState = PlayerState.Selecting;
    }

    //  Перемещение здания к указанной точке
    void MoveBuildingToPoint(Vector3 point)
    {
        //  Перемещение здания
        Vector3 convertedPoint = new Vector3((int)point.x, 0, (int)point.z);
        currentBuilding.transform.position = convertedPoint;

        //  Проверка касания препятствий
        if (!currentBuildingHelper.CheckCollisions())
            currentBuildingHelper.ChangeMaterial(GameManager.Instance.gameParams.errorMaterial);
    }
    
    /*-----Взаимодействие с UI-----*/

    //  Метод для открытия указанного на кнопке меню
    public void OpenMenu(RectTransform menu) => menu.gameObject.SetActive(!menu.gameObject.activeInHierarchy);
    
    //  Сохранение и выход в меню
    public void ExitToMainMenu()
    {
        SaveSystem.SaveProfile();
        SceneManager.LoadScene(0);
    }

}

