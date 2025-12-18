using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class QuestSystem : MonoBehaviour
{
    public static QuestSystem Instance; //  Синглтон для доступа

    List<Quest> activeQuests = new List<Quest>();
    float currentQuestLevel = 1; //  Уровень квестов, которые будут генерироваться игроку
    float maxQuestLevel = 5;
    int resourcesCount = 10;   //  Количество ресурсов, которое требуется
    float rewardMultiplier = 1.2f;  //  Множитель награды (награда = стоимость всех ресурсов, умноженная на множитель)
    public float CurrentQuestLevel  //  Свойство доступа к текущему уровню квестов
    {
        get=>currentQuestLevel;
        set => currentQuestLevel = Mathf.Clamp(value, 0, maxQuestLevel);    //  Уровень квестов не может быть выше максимального
    }
    public int CurrentQuestLevelInt //  Свойство доступа для получения округлённого значения уровня квестов
    {
        get => Mathf.FloorToInt(currentQuestLevel);
    }

    [SerializeField] UIQuestMenu questMenu; //  Меню квестов
    [SerializeField] AudioClip questRewardAudio;    //  Звук при получении награды

    //  Реализация Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);
    }

    //  Инициализация системы
    public void Init()
    {
        //  Если есть сохранение - загружаем предыдущий список квестов
        if (SaveSystem.currentPlayerProfile.questsList != null)
        {
            List<Quest> newQuestList = SaveSystem.currentPlayerProfile.questsList;
            foreach (Quest q in newQuestList)
                AddQuestToList(q);
            SaveSystem.currentPlayerProfile.questsList = activeQuests;
        }
        else
        {
            if (activeQuests.Count <= 0)
                for (int i = 0; i < questMenu.maxItemsCount; i++)
                {
                    CreateQuest();
                }
            SaveSystem.currentPlayerProfile.questsList = activeQuests;
        }
    }

    /*-----Методы для создания квестов-----*/
    // Метод для добавления квеста во все списки
    void AddQuestToList(Quest quest)
    {
        activeQuests.Add(quest);
        questMenu.AddNewQuest(quest);
    }
    //  Создание квеста
    void CreateQuest()
    {
        Quest newQuest = GenerateQuest();
        AddQuestToList(newQuest);
    }
    //  Генератор условий квеста
    public Quest GenerateQuest()
    {
        Quest newQuest = new Quest();
        List<Resource> resources = GenerateListOfResourcesForQuest(Mathf.FloorToInt(CurrentQuestLevelInt));
        List<Quest.QuestItem> questItemsList = new List<Quest.QuestItem>();

        foreach (Resource r in resources)
            questItemsList.Add(new Quest.QuestItem(r, resourcesCount));

        return new Quest(questItemsList,GetRewardFromListOfQuestItems(questItemsList),CurrentQuestLevelInt);
    }
    //  Генератор необходимых ресурсов для квеста в зависимости от уровня
    List<Resource> GenerateListOfResourcesForQuest(int level)
    {
        List<Resource> resourcesList = new List<Resource>();
        for (int i = 1; i <= level; i++)
        {
            Resource newResource;
            do
            {
                newResource = GetRandomResourceFromGameParams();
            }
            while (resourcesList.Contains(newResource) || !newResource.CanBeStored);
            resourcesList.Add(newResource);
        }

        return resourcesList;
    }
    //  Получение случайного ресурса из общего списка ресурсов
    Resource GetRandomResourceFromGameParams()
    {
        if (currentQuestLevel <= 2)
            return GameManager.Instance.gameParams.creatorRecipes[Random.Range(0, GameManager.Instance.gameParams.creatorRecipes.Count - 1)].OutResource.resource;
        else
            return GameManager.Instance.gameParams.resourcesList[Random.Range(0, GameManager.Instance.gameParams.resourcesList.Count - 1)];
    }
    //  Определение награды за квест в завимисости от используемых ресурсов
    int GetRewardFromListOfQuestItems(List<Quest.QuestItem> itemsList)
    {
        int reward = 0;
        foreach(Quest.QuestItem item in itemsList)
        {
            reward += item.resource.Cost * item.count;
        }
        return Mathf.FloorToInt(reward * rewardMultiplier);
    }

    //  Метод завершения квеста
    public void CompleteQuest(UIQuestPanel completedQuestPanel)
    {
        Quest completedQuest = completedQuestPanel.quest;
        if (activeQuests.Contains(completedQuest))
        {
            if (GameManager.Instance.TryTakeListOfResourceFromBank(completedQuest.questItems))
            {
                GameManager.Instance.AddMoney(completedQuest.reward);
                //  Повышение уровня квеста
                CurrentQuestLevel += 0.5f;
                //  Удаление квестов
                questMenu.RemoveQuest(completedQuestPanel);
                activeQuests.Remove(completedQuest);
                GameManager.Instance.PlaySound(questRewardAudio);
                //  Создание нового квеста
                CreateQuest();
            }
        }
    }
}
