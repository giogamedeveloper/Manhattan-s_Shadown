using UnityEngine;

public class DeathPlayerCars : MonoBehaviour
{
    public float sizeToImpact;
    // Update is called once per frame
    void Update()
    {
        NearToPlayer();
    }

    private void NearToPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, sizeToImpact, LayerMask.GetMask("Player"));
        if (colliders.Length > 0 && !GameController.Instance.gameOver)
        {
            GameController.Instance.GameOver();
        }
    }
}
