using UnityEngine;

[CreateAssetMenu(fileName = "NewStateTimerEndDecision", menuName = "State Machine/Decisions/ StateTimerEnd Decision")]
public class StateTimerEndDecision : Decision
{
    public override bool Decide(StateMachineController controller)
    {
        return CheckTimer(controller);
    }

    private bool CheckTimer(StateMachineController controller)
    {
        return controller.stateTimer <= 0f;
    }
}
