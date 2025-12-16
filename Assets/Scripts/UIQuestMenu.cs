using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIQuestMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] RectTransform questListPanel;
    [SerializeField] GameObject questPanelPrefab;
    List<UIQuestPanel> activeQuestPanelsList=new List<UIQuestPanel>();
    public int maxItemsCount = 3;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !PlayerController.Instance.isMouseOnUI)
            gameObject.SetActive(false);
    }

    public void AddNewQuest(Quest quest)
    {
        if (activeQuestPanelsList.Count < maxItemsCount)
        {
            UIQuestPanel newQuestPanel = Instantiate(questPanelPrefab, questListPanel).GetComponent<UIQuestPanel>();
            newQuestPanel.Init(quest);
            activeQuestPanelsList.Add(newQuestPanel);
        }
        else
            Debug.LogError("Trying to add new quest while there is no free space");
    }

    public void RemoveQuest(UIQuestPanel questPanel)
    {
        if (activeQuestPanelsList.Contains(questPanel))
        {
            activeQuestPanelsList.Remove(questPanel);
            Destroy(questPanel.gameObject);
        }
        else
            Debug.LogError("Trying to remove quest panel that is not in list");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayerController.Instance.isMouseOnUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PlayerController.Instance.isMouseOnUI = false;
    }
}
