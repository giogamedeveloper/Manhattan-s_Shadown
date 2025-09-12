using UnityEngine;

public class ReactionToogleInputs : Reaction
{
    [SerializeField] InputController _inputController;

    protected override void React()
    {
        _inputController.EnableUiInputs(true);
        _inputController.EnablePlayerInputs(false);
    }
}
