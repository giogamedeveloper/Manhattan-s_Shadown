using UnityEngine;

public class ShowReactionsLock : Reaction
{
    [SerializeField] GameObject[] _objetsReaction;

    protected override void React()
    {
        if (GameController.Instance.unLocked)
            foreach (GameObject reaction in _objetsReaction)
            {
                reaction.SetActive(true);
            }
    }
}
