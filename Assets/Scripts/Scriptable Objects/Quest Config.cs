using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest Config", menuName = "Configs/Quest Config",order =2)]
public class QuestConfig : ScriptableObject
{
    public List<Quest> quests;
}

[Serializable]
public class Quest
{
    public List<QuestItem> questItems;
    public int reward;
    public int level;

    [Serializable]
    public class QuestItem
    {
        public Resource resource;
        public int count;

        public QuestItem()
        {
            resource = null;
            count = 0;
        }
        public QuestItem(Resource resource, int count)
        {
            this.resource = resource;
            this.count = count;
        }
    }

    public Quest()
    {
        questItems = new List<QuestItem>();
        questItems.Add(new QuestItem());
        reward = 0;
        level = 1;
    }
    public Quest(List<QuestItem> itemsList, int reward, int level)
    {
        questItems = itemsList;
        this.reward = reward;
        this.level = level;
    }
}
