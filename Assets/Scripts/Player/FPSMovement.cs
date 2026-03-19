using System;
using UnityEngine;

public class FPSMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float stepDistance = 1f;

    [Header("Jump")]
    public float jumpForce = 4f;
    public float maxVerticalVelocity = 50f;
    public float highLandDistance = 2f;

    [Header("Virtual Sound")]
    public float walkStepRange = 5f;
    public float crouchWalkStepRange = .5f;
    public float landSoundRange = 8f;
    public float highLandSoundRange = 8f;

    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Animator _animator;
    public bool IsGrounded { get; private set; }

    public event Action OnStep;
    public event Action OnLand;
    public event Action OnHighLand;
    public event Action OnJumped;

    private float _moveHorizontal;
    private float _moveVertical;
    private float _movementMultiplier = 1f;
    private float _verticalVelocity;
    private float _stepDistanceCounter;
    private float _verticalDistanceCounter;
    private bool _groundedLastFrame;
    private bool _isCrouched;

    public void SetInput(float horizontal, float vertical)
    {
        _moveHorizontal = horizontal;
        _moveVertical = vertical;
    }

    public void SetCrouched(bool isCrouched, float multiplier)
    {
        _isCrouched = isCrouched;
        _movementMultiplier = multiplier;
    }

    public void Tick()
    {
        ApplyGravity();
        Land();
        Movement();
    }

    private void Movement()
    {
        Vector3 movement = new Vector3(_moveHorizontal, 0f, _moveVertical);
        movement = transform.rotation * movement;
        _characterController.Move(movement * (speed * Time.deltaTime * _movementMultiplier));

        float movementDistance = movement.magnitude * speed * Time.deltaTime * _movementMultiplier;
        float currentStepDistance = stepDistance * _movementMultiplier;

        if (IsGrounded && movementDistance > 0f)
        {
            if (_isCrouched) _animator.SetTrigger("CrouchWalking");
            else _animator.SetTrigger("Walk");
            _stepDistanceCounter += movementDistance;
        }
        else _stepDistanceCounter = 0f;

        if (_stepDistanceCounter >= currentStepDistance)
        {
            _stepDistanceCounter = 0f;
            OnStep?.Invoke();
            float soundRange = _isCrouched ? crouchWalkStepRange : walkStepRange;
            GenerateSound.Generate(transform.position, soundRange);
        }
    }

    private void ApplyGravity()
    {
        _characterController.Move(Vector3.up * (_verticalVelocity * Time.deltaTime));
        IsGrounded = _characterController.isGrounded;
        if (IsGrounded)
        {
            _verticalVelocity = Physics.gravity.y;
        }
        else
        {
            _verticalVelocity += Physics.gravity.y * Time.deltaTime;
            _verticalVelocity = Mathf.Clamp(_verticalVelocity, -maxVerticalVelocity, maxVerticalVelocity);
            if (_verticalVelocity <= 0f)
                _verticalDistanceCounter += Mathf.Abs(_verticalVelocity * Time.deltaTime);
        }
    }

    public void Jump()
    {
        if (IsGrounded)
        {
            _animator.SetTrigger("Jump");
            _verticalVelocity = jumpForce;
            OnJumped?.Invoke();
        }
    }

    private void Land()
    {
        if (IsGrounded && !_groundedLastFrame)
        {
            if (_verticalDistanceCounter >= highLandDistance)
            {
                OnHighLand?.Invoke();
                GenerateSound.Generate(transform.position, highLandSoundRange);
            }
            else
            {
                OnLand?.Invoke();
                GenerateSound.Generate(transform.position, landSoundRange);
            }
            _verticalDistanceCounter = 0f;
        }
        _groundedLastFrame = IsGrounded;
    }
}