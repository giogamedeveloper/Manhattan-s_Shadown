using UnityEngine;

public class InteractLock : Reaction
{
    private bool _isActive;

    protected override void React()
    {
        GameController.Instance.SwitchUIPlayer(_isActive);
        _isActive = !_isActive;
    }
}
