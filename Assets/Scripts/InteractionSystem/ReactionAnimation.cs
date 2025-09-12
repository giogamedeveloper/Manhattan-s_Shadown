using UnityEngine;

public class ReactionAnimation : Reaction
{
    public Animator target;
    public string triggerName;

    protected override void React()
    {
        target.SetTrigger(triggerName);
    }
}
