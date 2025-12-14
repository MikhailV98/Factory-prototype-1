using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIQuestPanel : MonoBehaviour
{
    public Quest quest;

    [SerializeField] RectTransform needResourcesPanel;
    [SerializeField] UIResourceImage moneyImagePanel;
    [SerializeField] GameObject resourceImageTemplate;
    [SerializeField] UIButtonProgressBar button;

    List<UIResourceImage> resourceImagesList = new List<UIResourceImage>();



    public void Init(Quest quest)
    {
        this.quest = quest;
        moneyImagePanel.Init(GameManager.Instance.gameParams.moneySprite, quest.reward.ToString());
        DisplayNeededResources();
        button.onSuccessClick.AddListener(() => QuestSystem.Instance.CompleteQuest(this));
        UpdateUI();
        ResourceStorage.onStorageUpdate.AddListener(UpdateUI);
    }

    void DisplayNeededResources()
    {
        if (quest == null)
        {
            Debug.LogError("Trying to display null quest");
            return;
        }

        if (quest.questItems.Count > 1)
        {
            List<RectTransform> panelsList = UIRecipeDisplayer.CreatePanels(2, needResourcesPanel);

            for (int i = 0; i < quest.questItems.Count; i++)
            {
                int numberInList = Mathf.FloorToInt((float)i / (float)3);
                GameObject newResourceImage = Instantiate(resourceImageTemplate, panelsList[numberInList]);
                UIResourceImage newResourceImageUI = newResourceImage.GetComponent<UIResourceImage>();
                newResourceImageUI.Init(quest.questItems[i]);
                resourceImagesList.Add(newResourceImageUI);
            }
        }
        else
        {
            List<RectTransform> panelsList = UIRecipeDisplayer.CreatePanels(1, needResourcesPanel);
            GameObject newResourceImage = Instantiate(resourceImageTemplate, panelsList[0]);
            UIResourceImage newResourceImageUI = newResourceImage.GetComponent<UIResourceImage>();
            newResourceImageUI.Init(quest.questItems[0]);
            resourceImagesList.Add(newResourceImageUI);
        }
    }
    void UpdateUI() 
    {
        int totalCountOfResources = 0;
        int totalCountNeeded = 0;
        for(int i = 0; i < quest.questItems.Count; i++)
        {
            int rNeeded = quest.questItems[i].count;
            int rCount = GameManager.Instance.GetResourceCountFromBank(quest.questItems[i].resource);
            rCount = Mathf.Clamp(rCount, 0, rNeeded);
            totalCountOfResources += rCount;
            totalCountNeeded += rNeeded;
            resourceImagesList[i].ChangeText(Mathf.Clamp(rCount, 0, rNeeded) + "/" + rNeeded);
            
        }
        button.Value = totalCountNeeded != 0 ? (float)totalCountOfResources / (float)totalCountNeeded : 0;
    }
}
