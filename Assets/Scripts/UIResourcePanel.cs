using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIResourcePanel : MonoBehaviour
{
    //  Панель отображения ресурсов

    [SerializeField] GameObject resourceWidget;
    [SerializeField] TextMeshProUGUI hideButtonText;
    [SerializeField] RectTransform resourceListPanel;
    RectTransform rectTransform;
    [SerializeField] TextMeshProUGUI storageText;
    [SerializeField] Slider storageBar;

    List<TextMeshProUGUI> resourceCountersList;


    ResourceStorage resourceBank;
    bool isHiden = false;

    private void Start()
    {
        rectTransform = transform as RectTransform;
    }

    //  Создание элементов панели
    //  Вызывается в методе Start менеджера
    public void Init(ResourceStorage storage)
    {
        resourceCountersList = new List<TextMeshProUGUI>();
        resourceBank = storage;
        foreach (Resource r in GameManager.Instance.gameParams.resourcesList)
            if(r.CanBeStored)
                AddResourcePanel(r);
        storage.ApplyResourcePanel(this);
    }
    void AddResourcePanel(Resource r)
    {
        GameObject newResourcePanel = Instantiate(resourceWidget, resourceListPanel);

        RectTransform newResourceTransform = newResourcePanel.transform as RectTransform;
        newResourceTransform.anchoredPosition = new Vector2(0, -25 - 50 * resourceCountersList.Count);

        newResourceTransform.Find("Image").GetComponent<Image>().sprite = r.Image;
        TextMeshProUGUI text = newResourceTransform.GetComponentInChildren<TextMeshProUGUI>();
        text.text = 0.ToString();
        resourceCountersList.Add(text);
        resourceBank.ApplyTextToStorage(r, text);
    }

    //  Скрытие и отображение панели
    public void OpenOrClosePanel()
    {
        if (isHiden)
        {
            ShowPanel();
            isHiden = false;
            hideButtonText.text = "<";
        }
        else
        {
            HidePanel();
            isHiden = true;
            hideButtonText.text = ">";
        }
    }
    //  Отображение и скрытие панели
    public void ShowPanel() => rectTransform.anchoredPosition = new Vector2(rectTransform.rect.width / 2, 60);
    public void HidePanel() => rectTransform.anchoredPosition = new Vector2(-rectTransform.rect.width / 2, 60);

    public void UpdateStorageBar(int count, int maxCount)
    {
        storageText.text = count + "/" + maxCount;
        storageBar.value = (float)count / (float)maxCount;
    }
}
