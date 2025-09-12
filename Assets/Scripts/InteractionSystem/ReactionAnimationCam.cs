using UnityEngine;

public class ReactionAnimationCam : Reaction
{
    public Animator target;
    bool isToggle = false;

    protected override void React()
    {
        target.SetBool("Toogle", isToggle);
        isToggle = !isToggle;
    }
}
