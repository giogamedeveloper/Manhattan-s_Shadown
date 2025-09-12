using UnityEngine;

public class HideObjectReaction : Reaction
{
    [SerializeField]
    GameObject objet;

    protected override void React()
    {
        objet.SetActive(false);
    }
}
