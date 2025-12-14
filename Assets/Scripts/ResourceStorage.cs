using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ResourceStorage
{
    public class StoredResource
    {
        public Resource resource;
        public int count;
        public TextMeshProUGUI counterText;
        public StoredResource(Resource r)
        {
            resource = r;
            count = 0;
        }
    }
    public static UnityEvent onStorageUpdate = new UnityEvent();
    List<StoredResource> resourceBank;
    UIResourcePanel resourcePanel;
    public int storageMaxCapacity = 50;

    public ResourceStorage(int maxCapacity)
    {
        resourceBank = new List<StoredResource>();

        foreach (Resource r in GameManager.Instance.gameParams.resourcesList)
        {
            if(r.CanBeStored)
                resourceBank.Add(new StoredResource(r));
        }
        storageMaxCapacity = maxCapacity;
    }

    public void AddResource(Resource r, int count)
    {
        StoredResource sr = FindResourceInBank(r);
        sr.count += count;
        UpdateResourcePanel(sr);
    }

    public bool TakeResource(Resource r, int count)
    {
        StoredResource sr = FindResourceInBank(r);
        if (sr.count >= count)
        {
            sr.count -= count;
            UpdateResourcePanel(sr);
            return true;
        }
        else
            return false;
    }
    public bool HasResource(Resource r, int count)
    {
        StoredResource sr = FindResourceInBank(r);
        return sr.count >= count;
    }
    void UpdateResourcePanel(StoredResource sr)
    {
        sr.counterText.text = sr.count.ToString();
        UpdateStorageSlider();
        onStorageUpdate.Invoke();
    }

    public int GetResourceCount(Resource r) => r.CanBeStored ? FindResourceInBank(r).count : 0;
    

    //  Поиск данных о ресурсе в банке.
    StoredResource FindResourceInBank(Resource r)
    {
        if (r.CanBeStored)
        {
            foreach (StoredResource sr in resourceBank)
            {
                if (sr.resource == r)
                    return sr;
            }
            StoredResource newSr = new StoredResource(r);
            resourceBank.Add(newSr);
            return newSr;
        }
        else return null;
    }

    public int GetTotalCount()
    {
        int count = 0;
        foreach (StoredResource sr in resourceBank)
            count += sr.count;
        return count;
    }

    public void UpdateStorageSlider()
        => resourcePanel.UpdateStorageBar(GetTotalCount(), storageMaxCapacity);

    public void ApplyTextToStorage(Resource r, TextMeshProUGUI tmp) 
        => FindResourceInBank(r).counterText = tmp;
    public void ApplyResourcePanel(UIResourcePanel newResourcePanel)
    { 
        resourcePanel = newResourcePanel;
        UpdateStorageSlider();
    }
}

