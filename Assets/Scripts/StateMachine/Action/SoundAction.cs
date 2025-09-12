using UnityEngine;

[CreateAssetMenu(fileName = "NewSoundAction", menuName = "State Machine/Actions/ SoundAction")]
public class SoundAction : StateAction
{
    public override void Act(StateMachineController controller)
    {
        FindSound(controller);
    }

    private void FindSound(StateMachineController controller)
    {
        if (controller.heardSounds.Count > 0)
        {
            //Tomamos el último y limpiamos el array
            controller.currentSoundPosition = controller.heardSounds[controller.heardSounds.Count - 1];
            controller.heardSounds.Clear();
        }
        controller.navMeshAgent.SetDestination(controller.currentSoundPosition);
        controller.navMeshAgent.isStopped = false;
        controller.navMeshAgent.speed = controller.stats.attackSpeed;
    }
}
