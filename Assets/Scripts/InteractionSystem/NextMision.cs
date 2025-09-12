using UnityEngine;

public class NextMision : Reaction
{
    protected override void React()
    {
        GameController.Instance.isActiveMision = true;
    }
}
