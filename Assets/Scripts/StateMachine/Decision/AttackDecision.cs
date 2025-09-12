using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackDecision", menuName = "State Machine/Decisions/Attack Decision")]
public class AttackDecision : Decision
{
    private bool IsCloseToPlayer(StateMachineController controller)
    {
        float distance = Vector3.Distance(controller.target.transform.position, controller.transform.position);
        return distance <= controller.stats.minAttackRange; // Define attackRange en tu controller
    }

    public override bool Decide(StateMachineController controller)
    {
        if (Arrived(controller))
            return false;

        if (IsCloseToPlayer(controller))
        {
            return true; 
        }

        return false;
    }

    private bool Arrived(StateMachineController controller)
    {
        return controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.stoppingDistance + 0.1f;
    }
}
