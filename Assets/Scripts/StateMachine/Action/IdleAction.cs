using UnityEngine;

[CreateAssetMenu(fileName = "NewAnimateAction", menuName = "State Machine/Actions/ IdleAction")]
public class IdleAction : StateAction
{
    public override void Act(StateMachineController controller)
    {
        controller.animator.SetFloat("SpeedV", 0f);
        controller.navMeshAgent.isStopped = true;
    }
}
