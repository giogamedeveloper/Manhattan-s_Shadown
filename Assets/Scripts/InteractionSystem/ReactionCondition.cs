using System;
using UnityEngine;

public class ReactionCondition : Reaction
{
    public string iDQuest;
    public string iDSubQuest;
    public bool conditionState;
    public static Action OnMisionCompleted;

    protected override void React()
    {
        switch (GameController.Instance.task)
        {
            case 1:
            {
                if (GameController.Instance.stateToTask == 3)
                {
                    LevelUpMision();
                    OnMisionCompleted?.Invoke();
                    GameController.Instance.task++;
                }
                break;
            }
            case 2:
            {
                LevelUpMision();
                GameController.Instance.stateToTask = -1;
                GameController.Instance.task++;


                break;
            }
            case 3:
            {
                if (GameController.Instance.stateToTask == 3)
                {
                    LevelUpMision();
                    GameController.Instance.task++;
                }
                break;
            }
            case 4:
            {
                if (GameController.Instance.unLocked)
                {
                    LevelUpMision();
                    GameController.Instance.task++;
                }
                break;
            }
            case 5:
            {
                LevelUpMision();
                break;
            }
        }
    }

    void LevelUpMision()
    {
        conditionState = true;
        DataManager.Instance.SetCondition(iDQuest, iDSubQuest, conditionState);
        if (DataManager.Instance.CheckCondition(iDQuest, iDSubQuest))
        {
            QuestManager.Instance.MarkSubQuestCompleted(iDQuest, iDSubQuest);
        }
    }
}
