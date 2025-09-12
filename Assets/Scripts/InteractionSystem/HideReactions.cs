using UnityEngine;

public class HideReactions : Reaction
{
    [SerializeField] GameObject[] _objetsReaction;

    protected override void React()
    {
        foreach (GameObject reaction in _objetsReaction)
        {
            reaction.SetActive(false);
        }
    }
}
