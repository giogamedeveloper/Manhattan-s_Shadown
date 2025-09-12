using UnityEngine;

public class ReactionCanvas : Reaction
{
    [SerializeField]
    private GameObject _canvasGroup;

    private bool isActive = true;

    protected override void React()
    {
        _canvasGroup.SetActive(isActive);
        isActive = !isActive;

    }
}
