using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    List<StoredResource> resourceBank;

    public ResourceStorage()
    {
        resourceBank = new List<StoredResource>();

        foreach (Resource r in GameManager.Instance.gameParams.resourcesList)
        {
            resourceBank.Add(new StoredResource(r));
        }
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
    void UpdateResourcePanel(StoredResource sr)
    {
        sr.counterText.text = sr.count.ToString();
    }

    public int GetResourceCount(Resource r)
    {
        return FindResourceInBank(r).count;
    }

    //  Поиск данных о ресурсе в банке.
    StoredResource FindResourceInBank(Resource r)
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

    public int GetTotalCount()
    {
        int count = 0;
        foreach (StoredResource sr in resourceBank)
            count += sr.count;
        return count;
    }

    public void ApplyTextToStorage(Resource r, TextMeshProUGUI tmp) 
        => FindResourceInBank(r).counterText = tmp;
}

