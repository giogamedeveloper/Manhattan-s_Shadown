using System.Collections.Generic;
using System.Linq;


[System.Serializable]
public class SubQuest
{
    public string subQuestID;
    public string description;
    public bool isCompleted;

    public void Complete()
    {
        isCompleted = true;
    }
}

[System.Serializable]
public partial class Quest
{
    public string questID;
    public string title;
    public List<SubQuest> subQuests;
    public bool isCompleted;

    public void CompleteSubQuest(int index)
    {
        if (index >= 0 && index < subQuests.Count)
        {
            subQuests[index].Complete();
            CheckQuestCompletion();
        }
    }

    private void CheckQuestCompletion()
    {
        isCompleted = subQuests.All(sq => sq.isCompleted);
    }
}
