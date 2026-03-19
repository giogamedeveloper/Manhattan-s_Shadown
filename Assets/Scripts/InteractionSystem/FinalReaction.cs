using System;
using UnityEngine;

public class FinalReaction : Reaction
{
    [SerializeField] GameObject gObject;

    public static Action OnCatched;
    public static Action OnAllMision;

    protected override void React()
    {
        if (!GameController.Instance.catched)
        {
            OnCatched?.Invoke();
            OnAllMision?.Invoke();
        }
        gObject.SetActive(true);
    }
}
