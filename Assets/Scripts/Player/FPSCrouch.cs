using System;
using UnityEngine;

public class FPSCrouch : MonoBehaviour
{
    public float regularHeight = 1.7f;
    public float crouchHeight = .5f;
    public float crouchMovementMultiplier = 0.5f;
    public float crouchTime = .5f;

    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Transform _head;
    [SerializeField] private Animator _animator;
    [SerializeField] private FPSMovement _movement;

    public bool IsCrouched { get; private set; }
    public event Action OnCrouched;

    private float _crouchTimer;

    public void ToggleCrouch()
    {
        if (IsCrouched && !CanStandUp()) return;
        IsCrouched = !IsCrouched;
        _crouchTimer = crouchTime;
        _movement.SetCrouched(IsCrouched, IsCrouched ? crouchMovementMultiplier : 1f);
        OnCrouched?.Invoke();
    }

    public void Tick()
    {
        if (_crouchTimer <= 0f) return;
        _crouchTimer -= Time.deltaTime;

        float initialHeight = _characterController.height;
        float targetHeight = IsCrouched ? crouchHeight : regularHeight;
        float targetCenterY = targetHeight / 2f;
        float t = 1 - _crouchTimer / crouchTime;

        _characterController.height = Mathf.Lerp(initialHeight, targetHeight, t);
        Vector3 center = _characterController.center;
        center.y = Mathf.Lerp(_characterController.center.y, targetCenterY, t);
        _characterController.center = center;

        Vector3 headPos = _head.localPosition;
        headPos.y = _characterController.height;
        _head.localPosition = headPos;

        _animator.SetBool("Crouch", IsCrouched);
    }

    private bool CanStandUp()
    {
        Vector3 initialPoint = transform.position + Vector3.up * crouchHeight;
        Vector3 endPoint = initialPoint + Vector3.up * (regularHeight - crouchHeight);
        return !Physics.Linecast(initialPoint, endPoint);
    }

    public void Initialize()
    {
        _characterController.height = regularHeight;
        Vector3 center = _characterController.center;
        center.y = regularHeight / 2f;
        _characterController.center = center;
        _movement.SetCrouched(false, 1f);
    }
}
