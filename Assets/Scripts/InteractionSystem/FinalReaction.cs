using System;
using UnityEngine;

public class FinalReaction : Reaction
{
    [SerializeField] GameManager script;

    public static Action OnCatched;
    public static Action OnAllMision;
    protected override void React()
    {
    if(!GameController.Instance.catched)
    {
        OnCatched?.Invoke();
        OnAllMision?.Invoke();
    }
        script.enabled = true;
    }
}
