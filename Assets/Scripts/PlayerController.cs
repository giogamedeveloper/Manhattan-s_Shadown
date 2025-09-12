using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static InputSystem_Actions;

public class PlayerController : MonoBehaviour, IPlayerActions
{
    // Referencia al InputController en escena
    private InputController inputController;

    // Variables para almacenar los inputs desde los callbacks
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpInput;
    private bool crouchInput;

    [Header("Variables")]
    private bool isPaused = true;

    private bool isCheats = false;

    [SerializeField] Light _lintern;
    private bool isActiveLintern = true;
    [SerializeField] Image _iconSprite;

    [Header("References")]
    [SerializeField] private CharacterController _characterController;

    [SerializeField] private DialogueTextContainer _dialogueText;

    [SerializeField] string _text;

    [SerializeField] Color _color;
    [SerializeField] private PlayerInteract _playerInteract;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _head;

    [Header("Map")]
    public GameObject fullMap;

    public GameObject miniMap;
    private bool isActiveMap = true;

    [Header("Movement")]
    public float speed = 5f;

    public float stepDistance = 1f;

    private float _moveHorizontal;
    private float _moveVertical;

    private Vector3 _movement = new Vector3();
    float _stepDistanceCounter;

    [Header("Rotation")]
    private float _rotationHorizontal;

    private float _rotationVertical;
    public float sesitivity = 2f;
    public bool invertView = false;
    public float verticalRotationLimit = 90;

    [Header("Jump")]
    public float jumpForce = 4f;

    public float maxVerticalVelocity = 50f;

    private float _verticalVelocity;

    [Header("Crouch")]
    private bool _isCrouched = false;

    public float regularHeight = 1.7f;
    public float crouchHeight = .5f;
    public float crouchMovementMultiplier = 0.5f;
    public float crouchTime = .5f;
    private float _crouchTimer;
    public float highLandDistance = 2f;

    private float _verticalDistanceCounter;
    private float _movementMultiplier = 1f;
    private bool _grounded;
    private bool _groundedLastFrame;

    [Header("Virtual Sound")]
    public float walkStepRange = 5f;

    public float crouchWalkStepRange = .5f;

    public float landSoundRange = 8f;

    public float highLandSoundRange = 8f;

    // Referencias a otras clases
    public Action OnStep;
    public Action OnLand;
    public Action OnHighLand;
    public Action OnJumped;
    public Action OnCrouched;

    void Start()
    {
        _characterController.height = regularHeight;
        Vector3 center = _characterController.center;
        center.y = regularHeight / 2f;
        _characterController.center = center;
    }

    void Update()
    {
        ApplyGravity();
        Land();
        Rotation();
        Movement();
        if (_crouchTimer > 0)
        {
            Crouch();
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.layer == LayerMask.NameToLayer("Limits"))
        {
            _text = TranslateManager.Instance.GetText(_text);
            _dialogueText.DisplayText(_text, _color);
        }
    }
    private void Movement()
    {
        _movement.Set(_moveHorizontal, 0f, _moveVertical);
        _movement = transform.rotation * _movement;
        _characterController.Move(_movement * (speed * Time.deltaTime * _movementMultiplier));

        float movementDistance = _movement.magnitude * speed * Time.deltaTime * _movementMultiplier;
        float currentStepDistance = stepDistance * _movementMultiplier;

        if (_grounded && movementDistance > 0f)
        {
            if (_isCrouched)
                _animator.SetTrigger("CrouchWalking");
            else
                _animator.SetTrigger("Walk");

            _stepDistanceCounter += movementDistance;
        }
        else
        {
            _stepDistanceCounter = 0f;
        }

        if (_stepDistanceCounter >= currentStepDistance)
        {
            _stepDistanceCounter = 0f;
            OnStep?.Invoke();
            float soundRange = _isCrouched ? crouchWalkStepRange : walkStepRange;
            GenerateSound.Generate(transform.position, soundRange);
        }
    }

    private void Rotation()
    {
        transform.Rotate(0f, _rotationHorizontal, 0f);
        _head.localEulerAngles = new Vector3(_rotationVertical, 0f, 0f);
    }

    private void ApplyGravity()
    {
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
                _verticalDistanceCounter += Mathf.Abs(_verticalVelocity * Time.deltaTime);
            }
        }
    }

    private void Jump()
    {
        if (_grounded)
        {
            _animator.SetTrigger("Jump");
            _verticalVelocity = jumpForce;
            OnJumped?.Invoke();
        }
    }

    private void Land()
    {
        if (_grounded && !_groundedLastFrame)
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
        _groundedLastFrame = _grounded;
    }

    private void Crouch()
    {
        {
            if (_crouchTimer > 0f)
            {
                _crouchTimer -= Time.deltaTime;
                float initialHeight = _characterController.height;
                float targetHeight = _isCrouched ? crouchHeight : regularHeight;

                float initialCenterY = _characterController.center.y;
                float targetCenterY = targetHeight / 2f;

                float t = 1 - _crouchTimer / crouchTime;

                _characterController.height = Mathf.Lerp(initialHeight, targetHeight, t);
                Vector3 center = _characterController.center;
                center.y = Mathf.Lerp(initialCenterY, targetCenterY, t);
                _characterController.center = center;

                Vector3 headPos = _head.localPosition;
                headPos.y = _characterController.height;
                _head.localPosition = headPos;

                _animator.SetBool("Crouch", _isCrouched);
            }
        }
    }

    void ToggleLintern()
    {
        isActiveLintern = !isActiveLintern;
        _lintern.enabled = !isActiveLintern;
        _iconSprite.sprite = isActiveLintern
            ? Resources.Load<Sprite>("Icons HUD/" + "linterna_OFF")
            : Resources.Load<Sprite>("Icons HUD/" + "linterna_ON");

    }

    // Métodos para la interfaz
    public void OnMove(InputAction.CallbackContext context)
    {
        if (GameController.Instance.movementInputActive)
        {
            moveInput = context.ReadValue<Vector2>();
            _moveHorizontal = moveInput.x;
            _moveVertical = moveInput.y;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (GameController.Instance.lookInputActive)
        {
            lookInput = context.ReadValue<Vector2>();
            _rotationHorizontal = lookInput.x * sesitivity;
            _rotationVertical += lookInput.y * sesitivity * (invertView ? 1 : -1);
            _rotationVertical = Mathf.Clamp(_rotationVertical, -verticalRotationLimit, verticalRotationLimit);

        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {

    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _playerInteract?.Interact();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && !_isCrouched)
            Jump();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            bool pause = GameController.Instance._isActive;
            GameController.Instance.TogglePause(pause);
        }
    }

    public void OnLintern(InputAction.CallbackContext context)
    {
        if (context.performed)
            ToggleLintern();
    }

    public void OnCheats(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameController.Instance.ToggleCheat(GameController.Instance.isCheatActive);
        }
    }

    public void OnMap(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            miniMap.SetActive(!isActiveMap);
            fullMap.SetActive(isActiveMap);
            isActiveMap = !isActiveMap;
        }
    }

    public void OnQuest(InputAction.CallbackContext context)
    {
        if (context.performed)
            QuestManager.Instance.ShowHideCanvas();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isCrouched = !_isCrouched; // cambio de estado
            _crouchTimer = crouchTime; // inicio la interpolación
        }
    }
}
