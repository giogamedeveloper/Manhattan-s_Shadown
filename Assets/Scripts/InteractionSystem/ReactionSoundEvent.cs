using System;
using UnityEngine;

public class ReactionSoundEvent : Reaction
{
    public static Action<string> OnReactionEventInvoke;

    public string clipNameDoor;

    protected override void React()
    {
        OnReactionEventInvoke?.Invoke(clipNameDoor);
    }
}
