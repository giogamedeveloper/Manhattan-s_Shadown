using UnityEngine;

[CreateAssetMenu(fileName = "NewNoTargetDecision", menuName = "State Machine/Decisions/No Target Decision")]
public class NoTargetDecision : Decision
{
    public override bool Decide(StateMachineController controller)
    {
        return CheckTarget(controller);
    }

    bool CheckTarget(StateMachineController controller)
    {
        return controller.target == null;
    }
}
