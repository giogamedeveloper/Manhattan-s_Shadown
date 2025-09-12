using System;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    public UnityEvent[] triggers;

    public void ExecuteTrigger(int triggerId)
    {
        try
        {
            triggers[triggerId].Invoke();
        }
        catch (Exception e)
        {
            Debug.LogWarning("No existe el índice del trigger: " + triggerId + e.Message);
            throw;
        }
    }
}
