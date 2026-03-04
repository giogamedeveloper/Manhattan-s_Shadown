using System.Collections.Generic;

[System.Serializable]
public class QuestDataWrapper
{
    public List<QuestData> quests = new();
}

[System.Serializable]
public class QuestData
{
    public string questID;
    public string title;
    public bool isCompleted;
    public List<SubQuestData> subQuests = new();
}

[System.Serializable]
public class SubQuestData
{
    public string subQuestID;
    public string description;
    public bool isCompleted;
}
