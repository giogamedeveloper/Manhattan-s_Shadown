using UnityEngine;

[CreateAssetMenu(fileName = "NewDecision", menuName = "State Machine/Decisions/Look Complex Decision")]
public class LookComplexDecision : Decision
{
    public LayerMask obstacleLayers;

    public override bool Decide(StateMachineController controller)
    {
        return Look(controller);
    }

    /// <summary>
    ///Identifica, mediante sphereCast, si tiene un objetivo a la vista. 
    /// </summary>
    /// <param name="controller"></param>
    /// <returns></returns>
    private bool Look(StateMachineController controller)
    {
        RaycastHit hit;
        Collider[] cols = Physics.OverlapSphere(controller.transform.position, controller.stats.reach,
            controller.stats.targetLayers);
        if (cols.Length > 0)
        {
            foreach (Collider collider in cols)
            {
                //Si se encuentra en el angulo de visión
                if (Vector3.Angle(collider.transform.position - controller.eyes.position, controller.transform.forward) <
                    controller.stats.fieldOfView / 2f)
                {
                    float rayDistance = Vector3.Distance(controller.eyes.position, collider.transform.position);
                    if (!Physics.Raycast(controller.eyes.position,
                        collider.transform.position + Vector3.up - controller.eyes.position, out hit,
                        rayDistance, obstacleLayers))
                    {
                        controller.target = collider.transform;
                        controller.lastSpottedPosition = collider.transform.position;
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
