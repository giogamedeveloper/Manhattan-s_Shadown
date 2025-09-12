using System;
using UnityEngine;

public class LightsOnOff : MonoBehaviour
{
    [SerializeField]
    private Transform _camera;

    [SerializeField] float distanceToActive;

    Light luz;

    void Start()
    {
        luz = GetComponent<Light>();
        if (luz == null)
        {
            Debug.LogError("Este objeto no tiene componente light");
        }
    }

    void Update()
    {
        if (luz == null) return;
        float distance = Vector3.Distance(_camera.position, transform.position);
        if (distance <= distanceToActive)
        {
            luz.enabled = true;
        }
        else
        {
            luz.enabled = false;
        }
    }
}
