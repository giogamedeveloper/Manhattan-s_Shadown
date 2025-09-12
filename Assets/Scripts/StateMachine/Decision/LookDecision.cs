using UnityEngine;

[CreateAssetMenu(fileName = "NewDecision", menuName = "State Machine/Decisions/Look Decision")]
public class LookDecision : Decision
{
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
        Debug.DrawRay(controller.eyes.position,
            controller.eyes.forward * controller.stats.reach,
            Color.green);
        Ray ray = new Ray(controller.eyes.position, controller.eyes.forward);
        if (Physics.SphereCast(ray,
            controller.stats.lookSphereCastRadius,
            out hit,
            controller.stats.reach,
            controller.stats.targetLayers))
        {
            controller.target = hit.transform;
            controller.lastSpottedPosition = hit.transform.position;
            return true;
        }
        return false;
    }
}
