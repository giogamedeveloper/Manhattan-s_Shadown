using UnityEngine;

[CreateAssetMenu(fileName = "New ChaseAction", menuName = "State Machine/Actions/ChaseAction")]
public class ChaseAction : StateAction
{
    public override void Act(StateMachineController controller)
    {
        Chase(controller);
    }

    /// <summary>
    ///Desplaza la IA hasta la última posición en la que vió al jugador 
    /// </summary>
    /// <param name="controller"></param>
    private void Chase(StateMachineController controller)
    {
        if (controller.target == null) return;
        controller.navMeshAgent.SetDestination(controller.lastSpottedPosition);
        controller.navMeshAgent.isStopped = false;
        controller.navMeshAgent.speed = controller.stats.attackSpeed;
        // Si llega hasta la última posición en la que avistó al objetivo sin que haya saltado a otro estado (araque, etc...) significará que ya, probablemente, no está viendo al objetivo y lo ha perdido.

        if (controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.stoppingDistance)
        {
            controller.target = null;
        }
    }
}
