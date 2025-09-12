using UnityEngine;
using static InputSystem_Actions;

public class InputController : MonoBehaviour
{
    [SerializeField]
    private GameObject playerActionsGO;

    [SerializeField]
    private GameObject uIActionsGO;

    public InputSystem_Actions _inputs;
    private IPlayerActions _playerActions;
    private IUIActions _uiActions;

    void OnValidate()
    {
        if (playerActionsGO != null && !playerActionsGO.TryGetComponent(out _playerActions)) playerActionsGO = null;
        if (uIActionsGO != null && !uIActionsGO.TryGetComponent(out _uiActions)) uIActionsGO = null;
    }

    void Awake()
    {
        _inputs = new InputSystem_Actions();
        if (_playerActions == null && playerActionsGO != null)
            _playerActions = playerActionsGO.GetComponent<IPlayerActions>();
        if (_uiActions == null && uIActionsGO != null)
            _uiActions = uIActionsGO.GetComponent<IUIActions>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_playerActions != null)
        {
            _inputs.Player.AddCallbacks(_playerActions);
        }
        if (_uiActions != null)
        {
            _inputs.UI.AddCallbacks(_uiActions);
        }
        _inputs.Player.Enable();
        _inputs.UI.Disable();
    }

    public void EnablePlayerInputs(bool value)
    {
        Debug.Log("EnablePlayerInputs");
        if (value)
            _inputs.Player.Enable();
        else
            _inputs.Player.Disable();
    }

    public void EnableUiInputs(bool value)
    {
        Debug.Log("Enable UI Inputs");
        if (value)
            _inputs.UI.Enable();
        else
            _inputs.UI.Disable();
    }
}
