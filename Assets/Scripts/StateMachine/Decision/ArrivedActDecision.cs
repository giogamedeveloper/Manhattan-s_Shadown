using UnityEngine;

[CreateAssetMenu(fileName = "NewArrivedActDecision", menuName = "State Machine/Decisions/Arrived Act Decision")]
public class ArrivedActDecision : Decision
{
    public override bool Decide(StateMachineController controller)
    {
        return Arrived(controller);
    }

    private bool Arrived(StateMachineController controller)
    {
        return controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.stoppingDistance;
    }
}
