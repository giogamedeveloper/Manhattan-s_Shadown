using UnityEngine;

[CreateAssetMenu(fileName = "New State", menuName = "State Machine/New State")]
public class State : ScriptableObject
{
    public StateAction[] startActions;
    public StateAction[] updateActions;
    public StateAction[] endActions;

    //Listado de las transiciones posibles a otros estados
    public Transition[] transitions;

    /// <summary>
    /// Ejecuta la lista de acciones recibidas como parámetro.
    /// </summary>
    /// <param name="controller"></param>
    public void StartState(StateMachineController controller)
    {
        DoActions(controller, startActions);
        CheckTransitions(controller);
    }

    public void UpdateState(StateMachineController controller)
    {
        DoActions(controller, updateActions);
        CheckTransitions(controller);
    }

    public void EndState(StateMachineController controller)
    {
        DoActions(controller, endActions);
    }


    /// <param name="actions"></param>
    private void DoActions(StateMachineController controller, StateAction[] actions)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].Act(controller);
        }
    }


    private void CheckTransitions(StateMachineController controller)
    {
        for (int i = 0; i < transitions.Length; i++)
        {
            bool decisionSucceded = transitions[i].decision.Decide(controller);
            if (decisionSucceded && transitions[i].trueState != null)
            {
                controller.TransitionToState(transitions[i].trueState);
                break;
            }
            else if (!decisionSucceded && transitions[i].falseState != null)
            {
                controller.TransitionToState(transitions[i].falseState);
                break;
            }
        }
    }
}
