using UnityEngine;

public class ChangeStateReaction : Reaction
{
    [SerializeField]
    StateMachineController stateMachineController;

    [SerializeField]
    State state;

    protected override void React()
    {
        stateMachineController.TransitionToState(state);
    }
}
