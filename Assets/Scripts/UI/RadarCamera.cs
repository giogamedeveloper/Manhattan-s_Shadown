using System;
using UnityEngine;

public class RadarCamera : MonoBehaviour
{
    //Objeto de enfoque de la cámara
    public Transform target;

    //campo de viusion del radar
    public float fov = 20f;

    //Variable para referenciar la referencia al componente camera
    private Camera _cam;


    void Awake()
    {
        if (_cam == null)
            _cam = GetComponent<Camera>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        //Posicionamiento de la cámara en el objetivo y lo elevamos en función de fov
        transform.position = target.position + Vector3.up * fov;
        //Para poder usar el radar el modo ortografico
        _cam.orthographicSize = fov;
    }
}
