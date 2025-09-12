using System;
using UnityEngine;

public class ControlLock : MonoBehaviour
{
    int[] result, correctCombination;
    [SerializeField] Animator lockAnimator;
    [SerializeField] Animator chestAnimator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        result = new int[] { 0, 0, 0 };
        correctCombination = new int[] { 1, 0, 4 };
        RotateLock.Rotate += CheckResults;
    }

    private void CheckResults(string wheelName, int number)
    {
        switch (wheelName)
        {
            case "FirstGear":
                result[0] = number;
                Debug.Log(number);
                break;
            case "SecondGear":
                result[1] = number;
                break;
            case "ThirdGear":
                result[2] = number;
                break;
        }
        if (result[0] == correctCombination[0] && result[1] == correctCombination[1] && result[2] == correctCombination[2])
        {
            lockAnimator.SetTrigger("Open");
            chestAnimator.SetTrigger("Open");
            GameController.Instance.LockOn();
        }
    }

    void OnDestroy()
    {
        RotateLock.Rotate -= CheckResults;
    }
}
