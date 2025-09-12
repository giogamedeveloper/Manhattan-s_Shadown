using UnityEngine;

[CreateAssetMenu(fileName = "NewHearSoundDecision", menuName = "State Machine/Decisions/HearSound Decision")]
public class HearSoundDecision : Decision
{
    public override bool Decide(StateMachineController controller)
    {
        return HearSound(controller);
    }

    private bool HearSound(StateMachineController controller)
    {
        return controller.heardSounds.Count > 0;
    }
    
    
}
