using UnityEngine;

public class ShowReactions : Reaction
{
    [SerializeField] GameObject[] _objetsReaction;

    protected override void React()
    {
            foreach (GameObject reaction in _objetsReaction)
            {
                reaction.SetActive(true);
            }
    }
}
