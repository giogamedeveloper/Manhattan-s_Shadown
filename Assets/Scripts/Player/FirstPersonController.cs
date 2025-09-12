using System;
using Unity.VisualScripting;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public Action OnStep;
    public Action OnLand;
    public Action OnHighLand;
    public Action OnJump;
    public Action OnCrouch;


    [Header("References")]
    [SerializeField]
    private CharacterController _characterController;

    [SerializeField]
    private PlayerInteract _playerInteract;

    [SerializeField]
    private Animator _animator;

    [Header("Movement")]
    public float speed = 5f;

    //
    public float stepDistance = 1f;

    //Variables para ir almacenando la direccion
    private float _moveHorizontal;

    private float _moveVertical;

    //Vector para almacenar el movimiento actual
    private Vector3 _movement = new Vector3();

    //
    float _stepDistanceCounter;

    [Header("Rotation")]
    //Variables para almacenar la rotacion segun los inputs
    private float _rotationHorizontal;

    private float _rotationVertical;

    public float sesitivity = 2f;
    [SerializeField] Transform _head;
    public bool invertView = false;

    [Range(0f, 90f)]
    public float verticalRotationLimit = 90;

    [Header("Jump")]
    public float jumpForce = 4f;

    [Range(20f, 50f)]
    public float maxVerticalVelocity = 50f;

    public float minLandDistance = .5f;
    public float highLandDistance = 2f;

    private float _verticalVelocity;

    float _verticalDistanceCounter;

    [Header("Crouch")]
    private bool _isCrouched = false;

    public float regularHeight = 1.7f;
    public float crouchHeight = .5f;

    [Range(.2f, 1f)]
    public float crouchMovementMultiplier = 0.5f;

    public float crouchTime = .5f;

    private float _crouchTimer;

    float _movementMultiplier = 1f;

    bool _grounded;
    bool _groundedLastFrame;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _characterController.height = regularHeight;
        Vector3 center = _characterController.center;
        center.y = regularHeight / 2f;
        _characterController.center = center;
    }

    // Update is called once per frame
    void Update()
    {
        ApplyGravity();
        Land();
        Controls();
        Rotation();
        Movement();
        Crouch();
    }

    void LateUpdate()
    {
        _groundedLastFrame = _grounded;
    }

    private void Controls()
    {
        _moveHorizontal = Input.GetAxisRaw("Horizontal");
        _moveVertical = Input.GetAxisRaw("Vertical");

        _rotationHorizontal = Input.GetAxisRaw("Mouse X") * sesitivity;
        _rotationVertical += Input.GetAxisRaw("Mouse Y") * sesitivity * (invertView ? 1 : -1);
        _rotationVertical = Mathf.Clamp(_rotationVertical, -verticalRotationLimit, verticalRotationLimit);

        if (!_isCrouched && Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        if (Input.GetButtonDown("Crouch") && ((_isCrouched && CanStandUp()) || !_isCrouched))
        {
            OnCrouch?.Invoke();
            _isCrouched = !_isCrouched;
            _movementMultiplier = _isCrouched ? crouchMovementMultiplier : 1;
            _crouchTimer = (_crouchTimer > 0f) ? (crouchTime - _crouchTimer) : crouchTime;
        }
        if (Input.GetButtonUp("Interact"))
        {
            _playerInteract.Interact();
        }
    }

    private void Movement()
    {
        //Modificamos movement para añadirle la direccion de cada eje
        _movement.Set(_moveHorizontal, 0f, _moveVertical);
        //Orientamos el vector de movimiento según la rotación de la cámara en Y (un quaternion por otro vector da la rotacion )
        _movement = transform.rotation * _movement;
        //Aplicamos el movimiento a traves de character controller mediante el metodo Move
        _characterController.Move(_movement * (speed * Time.deltaTime * _movementMultiplier));


        float movementDistance = _movement.magnitude * speed * Time.deltaTime * _movementMultiplier;
        float currentStepDistance = stepDistance * _movementMultiplier;
        //Incremento distancia si está en el suelo sino reinicio el estado
        if (_grounded && movementDistance > 0f)
        {
            if (_isCrouched)
            {
                _animator.SetTrigger("CrouchWalking");
                
            }
            else
            {
                _animator.SetTrigger("Walk");
            }
            _stepDistanceCounter += movementDistance;
        }
        else
        {
            _stepDistanceCounter = 0f;
        }
        //Si la distancia alcanza el valor deseado, reiniciamos el contador y ejecutamos el paso
        if (_stepDistanceCounter >= currentStepDistance)
        {
            _stepDistanceCounter = 0f;
            OnStep?.Invoke();
        }
    }

    private void Rotation()
    {

        transform.Rotate(0f, _rotationHorizontal, 0f);
        _head.localEulerAngles = new Vector3(_rotationVertical, 0f, 0f);
    }

    private void ApplyGravity()
    {
        //Aplicamos la fuerza vertical
        _characterController.Move(Vector3.up * (_verticalVelocity * Time.deltaTime));

        _grounded = _characterController.isGrounded;
        if (_grounded)
        {
            _verticalVelocity = Physics.gravity.y;
        }
        else
        {
            _verticalVelocity += Physics.gravity.y * Time.deltaTime;
            _verticalVelocity = Mathf.Clamp(_verticalVelocity, -maxVerticalVelocity, maxVerticalVelocity);
            if (_verticalVelocity <= 0f)
            {
                _verticalDistanceCounter += MathF.Abs(_verticalVelocity * Time.deltaTime);
            }
        }
    }

    private void Jump()
    {
        if (_grounded)
        {
            _animator.SetTrigger("Jump");
            _verticalVelocity = jumpForce;
            OnJump?.Invoke();
        }
    }

    private void Land()
    {
        if (_grounded && !_groundedLastFrame)
        {
            if (_verticalDistanceCounter >= highLandDistance)
            {
                OnHighLand?.Invoke();
            }
            else
            {
                OnLand?.Invoke();
            }
            _verticalDistanceCounter = 0f;
        }
    }

    private void Crouch()
    {
        if (_crouchTimer <= 0f) return;
        _crouchTimer -= Time.deltaTime;

        float initialHeight = _isCrouched ? regularHeight : crouchHeight;

        float targetHeight = _isCrouched ? crouchHeight : regularHeight;

        float initialCenter = initialHeight / 2f;
        float targetCenter = targetHeight / 2f;

        //AL ir decrementando el tiempo inicial de agachado se pone 1- para q vaya de 0 to 1
        float t = 1 - _crouchTimer / crouchTime;

        _characterController.height = Mathf.Lerp(initialHeight, targetHeight, t);
        Vector3 center = _characterController.center;
        center.y = Mathf.Lerp(initialCenter, targetCenter, t);
        _characterController.center = center;

        Vector3 headPosition = _head.localPosition;
        headPosition.y = _characterController.height;
        _head.localPosition = headPosition;
        _animator.SetBool("Crouch", _isCrouched);

    }

    private bool CanStandUp()
    {
        Vector3 initialPoint = transform.position + Vector3.up * crouchHeight;
        Vector3 endPoint = initialPoint + Vector3.up * (regularHeight - crouchHeight);
        bool canStandUp = !Physics.Linecast(initialPoint, endPoint);

        return canStandUp;
    }
}
