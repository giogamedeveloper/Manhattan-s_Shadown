using UnityEngine;

public class CheckAndCountItems : Reaction
{
    protected override void React()
    {
        GameController.Instance.stateToTask++;
        if (GameController.Instance.stateToTask > 3)
        {
            Debug.Log("Cumplo condicion");
        }
        Destroy(gameObject);
    }
}
