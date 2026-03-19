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

    [SerializeField] private FPSRotation _rotation;
    [SerializeField] private FPSCrouch _crouch;
    [SerializeField] private FPSMovement _fpsMovement;
    public FPSMovement Movement
    {
        get { return _fpsMovement; }
    }
    [SerializeField] private DialogueTextContainer _dialogueText;
    [SerializeField] string _text;

    [SerializeField] Color _color;
    [SerializeField] private PlayerInteract _playerInteract;

    [Header("Map")]
    public GameObject fullMap;

    public GameObject miniMap;
    private bool isActiveMap = true;
    public float regularHeight = 1.7f;

    void Start()
    {
        _characterController.height = regularHeight;
        Vector3 center = _characterController.center;
        center.y = regularHeight / 2f;
        _characterController.center = center;
        _crouch.Initialize();
    }

    void Update()
    {
        _fpsMovement.Tick();
        _crouch.Tick();
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.layer == LayerMask.NameToLayer("Limits"))
        {
            _text = TranslateManager.Instance.GetText(_text);
            _dialogueText.DisplayText(_text, _color);
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
            _fpsMovement.SetInput(moveInput.x, moveInput.y);
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (GameController.Instance.lookInputActive)
        {
            lookInput = context.ReadValue<Vector2>();
            _rotation.AddRotation(lookInput.x * _rotation.sensitivity, lookInput.y * _rotation.sensitivity);
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
        if (context.performed && !_crouch.IsCrouched)
            _fpsMovement.Jump();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            bool pause = !GameController.Instance._isActive;
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
            _crouch.ToggleCrouch();
    }
}
