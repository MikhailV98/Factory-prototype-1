using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSystem : MonoBehaviour
{
    public static QuestSystem Instance;

    List<Quest> activeQuests = new List<Quest>();
    float currentQuestLevel = 1; //  ”ровень квестов, которые будут генерироватьс€ игроку
    float maxQuestLevel = 5;
    int resourcesCount = 10;   //   оличество ресурсов, которое требуетс€
    float rewardMultiplier = 1.2f;
    public float CurrentQuestLevel
    {
        get=>currentQuestLevel;
        set => currentQuestLevel = Mathf.Clamp(value, 0, maxQuestLevel);
    }
    public int CurrentQuestLevelInt
    {
        get => Mathf.FloorToInt(currentQuestLevel);
    }

    [SerializeField] UIQuestMenu questMenu;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            Init();
        }
        else
            Destroy(this);
    }

    public void Init()
    {
        if (activeQuests.Count <= 0)
            for (int i = 0; i < questMenu.maxItemsCount; i++)
            {
                CreateQuest();
            }
    }
    void CreateQuest()
    {
        Quest newQuest = GenerateQuest();
        activeQuests.Add(newQuest);
        questMenu.AddNewQuest(newQuest);
    }

    public Quest GenerateQuest()
    {
        Quest newQuest = new Quest();
        List<Resource> resources = GenerateListOfResourcesForQuest(Mathf.FloorToInt(CurrentQuestLevelInt));
        List<Quest.QuestItem> questItemsList = new List<Quest.QuestItem>();

        foreach (Resource r in resources)
            questItemsList.Add(new Quest.QuestItem(r, resourcesCount));

    
        return new Quest(questItemsList,GetRewardFromListOfQuestItems(questItemsList),CurrentQuestLevelInt);
    }

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
    Resource GetRandomResourceFromGameParams()
    {
        if (currentQuestLevel <= 2)
            return GameManager.Instance.gameParams.creatorRecipes[Random.Range(0, GameManager.Instance.gameParams.creatorRecipes.Count - 1)].OutResource.resource;
        else
            return GameManager.Instance.gameParams.resourcesList[Random.Range(0, GameManager.Instance.gameParams.resourcesList.Count - 1)];
    }
    int GetRewardFromListOfQuestItems(List<Quest.QuestItem> itemsList)
    {
        int reward = 0;
        foreach(Quest.QuestItem item in itemsList)
        {
            reward += item.resource.Cost * item.count;
        }
        return Mathf.FloorToInt(reward * rewardMultiplier);
    }

    public void CompleteQuest(UIQuestPanel completedQuestPanel)
    {
        Quest completedQuest = completedQuestPanel.quest;
        if (activeQuests.Contains(completedQuest))
        {
            if (GameManager.Instance.TryTakeListOfResourceFromBank(completedQuest.questItems))
            {
                GameManager.Instance.AddMoney(completedQuest.reward);
                //  ѕовышение уровн€ квеста
                CurrentQuestLevel += 0.5f;
                questMenu.RemoveQuest(completedQuestPanel);
                activeQuests.Remove(completedQuest);

                CreateQuest();
            }
        }
    }
}
