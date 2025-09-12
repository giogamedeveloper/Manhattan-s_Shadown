using UnityEngine;

public class GenerateSound
{
    public static void Generate(Vector3 position, float range)
    {
        Collider[] cols = Physics.OverlapSphere(position, range);
        foreach (Collider col in cols)
        {
            if (col.TryGetComponent(out StateMachineController controller))
            {
                controller.HearSound(position);
            }
        }
    }
}
