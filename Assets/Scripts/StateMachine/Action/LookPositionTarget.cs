using UnityEngine;

[CreateAssetMenu(fileName = "NewLookPositionTarget", menuName = "State Machine/Actions/LookPosition Target")]
public class LookPositionTarget : StateAction
{
    //Velocidad de giro
    public float turnSpeed = 4f;

    //Tiempo que dura el estado
    public float lostDuration = 2f;

    public override void Act(StateMachineController controller)
    {
        Look(controller);
    }

    private void Look(StateMachineController controller)
    {
        //Verificamos si no se ha inicicado el contador antes
        if (!controller.isTimerCounting)
        {
            //De no ser así, lo iniciamos 
            controller.isTimerCounting = true;
            controller.stateTimer = lostDuration;
        }
        //Paramos el movimiento y rotamos hacia donde se encuentra el jugador tratando de localizarlo
        controller.navMeshAgent.isStopped = true;
        if (controller.target != null)
        {
            Vector3 targetToLook = controller.navMeshAgent.destination;
            Quaternion targetRotation =
                Quaternion.LookRotation(Vector3.ProjectOnPlane(targetToLook - controller.eyes.position,
                    Vector3.up));
            controller.transform.rotation = Quaternion.Slerp(controller.transform.rotation, targetRotation,
                Time.deltaTime * turnSpeed);
        }
        //Seguimos descontando el tiempo
        controller.stateTimer -= Time.deltaTime;
        if (controller.stateTimer <= 0)
        {
            controller.target = null;
        }
    }
}
