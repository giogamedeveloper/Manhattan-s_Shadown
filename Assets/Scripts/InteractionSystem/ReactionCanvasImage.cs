using UnityEngine;

public class ReactionCanvasImage : Reaction
{
    [SerializeField]
    private CanvasGroup _canvasGroup;

    protected override void React()
    {
        GameController.Instance._noteCanvasGroup = _canvasGroup;
        GameController.Instance.movementInputActive = false;
        GameController.Instance.lookInputActive = false;
        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        
    }
}
