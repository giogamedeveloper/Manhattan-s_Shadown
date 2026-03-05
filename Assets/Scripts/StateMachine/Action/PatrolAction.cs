using UnityEngine;

[CreateAssetMenu(fileName = "PatrolAction", menuName = "State Machine/PatrolAction")]
public class PatrolAction : StateAction
{
    public bool randomPatrol = false;

    public override void Act(StateMachineController controller)
    {
        Patrol(controller);
    }

    /// <summary>
    /// Realiza las acciones de cambio de waypoint y gestión del navmesh para la patrulla
    /// </summary>
    /// <param name="controller"></param>
    private void Patrol(StateMachineController controller)
    {
        //Recuperamos el siguiente destino desde la lista de destinos del controller.
        controller.navMeshAgent.SetDestination(controller.NextWayPoint);
        //Hacemos que se mueva
        controller.navMeshAgent.isStopped = false;
        // Si la distancia restante al destino es inferior al stopping distance configurado y no hay camino pendiente de calcular, consideramos que hemos llegado al destino y seleccionamos un nuevo destino.
        if (controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.stoppingDistance &&
            !controller.navMeshAgent.pathPending)
        {
            if (randomPatrol)
            {
                controller.nextWayPoint = Random.Range(0, controller.wayPointsList.Count);
            }
            else
            {
                //En caso contrario elegimos el siguiente de la lista; mediante este truco con el módulo, Nos aseguramos de que no se desborde el índice de la lista.
                controller.nextWayPoint = (controller.nextWayPoint + 1) % controller.wayPointsList.Count;
            }
        }
    }
}
