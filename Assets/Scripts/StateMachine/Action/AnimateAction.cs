using UnityEngine;

[CreateAssetMenu(fileName = "NewAnimateAction", menuName = "State Machine/Actions/ AnimateAction")]
public class AnimateAction : StateAction
{
    public enum ParamType
    {
        Trigger,
        Float
    }

    public ParamType type;
    public string animationParam;

    public override void Act(StateMachineController controller)
    {
        Animate(controller);
    }

    private void Animate(StateMachineController controller)
    {
        if (type == ParamType.Trigger)
        {
            controller.animator.SetTrigger(animationParam);
            GameController.Instance.catched = true;
            if (!GameController.Instance.gameOver)
                GameController.Instance.GameOver();
        }
        else if (type == ParamType.Float)
        {
            controller.animator.SetFloat(animationParam, controller.navMeshAgent.velocity.magnitude);
        }
    }
}
