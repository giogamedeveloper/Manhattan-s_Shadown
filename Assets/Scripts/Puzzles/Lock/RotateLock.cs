using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateLock : MonoBehaviour
{
    public static event Action<string, int> Rotate = delegate { };

    bool _coroutineAllowed;
    int _numberShown;

    // Define el layer en el que quieres detectar clics
    public LayerMask clickableLayer;

    private Camera mainCamera;

    void Start()
    {
        _coroutineAllowed = true;
        _numberShown = 0;
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, clickableLayer))
            {
                // Aquí verificas si el objeto golpeado está en el layer correcto
                if (hit.transform == this.transform)
                {
                    if (_coroutineAllowed)
                    {
                        StartCoroutine(RotateWheel());
                    }
                }
            }
        }
    }

    IEnumerator RotateWheel()
    {
        _coroutineAllowed = false;
        for (int i = 0; i < 11; i++)
        {
            transform.Rotate(0f, 3.3f, 0f);
            yield return new WaitForSeconds(0.01f);
        }
        _coroutineAllowed = true;
        _numberShown += 1;
        if (_numberShown > 9)
        {
            _numberShown = 0;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        Rotate(name, _numberShown);
    }
}
