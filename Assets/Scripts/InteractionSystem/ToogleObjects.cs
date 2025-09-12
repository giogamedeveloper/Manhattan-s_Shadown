using UnityEngine;

public class ToogleObjects : Reaction
{
    [SerializeField]
    GameObject[] objets;

    protected override void React()
    {
        foreach (GameObject obj in objets)
        {
            obj.SetActive(true);
        }
    }
}
