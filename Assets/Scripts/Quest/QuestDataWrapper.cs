using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestDataWrapper
{
    public List<QuestData> quests;
}

[System.Serializable]
public class QuestData
{
    public string title;
    public bool isCompleted;
    public List<SubQuestData> subQuests;
}

[System.Serializable]
public class SubQuestData
{
    public string description;
    public bool isCompleted;
}
