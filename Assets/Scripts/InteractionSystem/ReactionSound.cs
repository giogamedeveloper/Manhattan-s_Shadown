using UnityEngine;
using UnityEngine.Serialization;

public class ReactionSound : Reaction
{
    public string soundDoorName;

    protected override void React()
    {
        AudioController.Instance.PlaySound(soundDoorName);
    }
}
