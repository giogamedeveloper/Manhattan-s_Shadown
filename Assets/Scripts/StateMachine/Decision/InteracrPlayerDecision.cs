using UnityEngine;

[CreateAssetMenu(fileName = "NewDecision", menuName = "State Machine/Decisions/Interact Player Decision")]
public class InteractPlayerDecision : Decision
{
    public override bool Decide(StateMachineController controller)
    {
        return StateIdle(controller);
    }

    /// <summary>
    ///Identifica, mediante sphereCast, si tiene un objetivo a la vista. 
    /// </summary>
    /// <param name="controller"></param>
    /// <returns></returns>
    private bool StateIdle(StateMachineController controller)
    {
        if (GameController.Instance.orientToPlayer)
            return true;
        return false;
    }
}
