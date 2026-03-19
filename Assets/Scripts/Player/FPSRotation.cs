using UnityEngine;

public class FPSRotation : MonoBehaviour
{
    [SerializeField] private Transform _head;
    public float sensitivity = 2f;
    public bool invertView = false;
    [Range(0f, 90f)] public float verticalRotationLimit = 90f;

    private float _rotationVertical;

    public void AddRotation(float horizontal, float vertical)
    {
        transform.Rotate(0f, horizontal, 0f);
        _rotationVertical += vertical * (invertView ? 1 : -1);
        _rotationVertical = Mathf.Clamp(_rotationVertical, -verticalRotationLimit, verticalRotationLimit);
        _head.localEulerAngles = new Vector3(_rotationVertical, 0f, 0f);
    }
}
