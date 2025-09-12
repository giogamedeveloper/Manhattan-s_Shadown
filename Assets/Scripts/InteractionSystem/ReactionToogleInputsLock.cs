using UnityEngine;

public class ReactionToogleInputsLock : Reaction
{
    [SerializeField] InputController _inputController;

    protected override void React()
    {
        GameController.Instance.movementInputActive = false;
        GameController.Instance.lookInputActive = false;
        // _inputController.EnableUiInputs(true);
        // _inputController.EnablePlayerInputs(false);
    }
}
