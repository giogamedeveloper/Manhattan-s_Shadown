using UnityEngine;

public class OrientObjectReaction : Reaction
{
    [SerializeField]
    private GameObject originObject;

    [SerializeField]
    private GameObject destinyObject;


    protected override void React()
    {
        Vector3 direct = destinyObject.transform.position - originObject.transform.position;
        originObject.gameObject.transform.forward = direct.normalized;
        GameController.Instance.orientToPlayer = true;
        Debug.Log("OrientObjectReaction React");
    }
}
