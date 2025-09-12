using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static InputSystem_Actions;


public class GameController : MonoBehaviour, IUIActions
{
    [Header("References")]
    public QuestManager questManager;

    [SerializeField] InputController _inputController;

    [SerializeField] PlayTimeTracker timeToPlaying;
    public PauseMenu pauseMenu;

    [Header("Canvas")]
    [SerializeField]
    private CanvasGroup cheatCanvasGroup;

    public CanvasGroup _noteCanvasGroup;

    [SerializeField]
    private CanvasGroup endGameCanvasGroup;

    [SerializeField] CanvasGroup winGame;
    [SerializeField] GameObject minimapCanvas;
    [SerializeField] CanvasGroup hudCanvas;

    [HideInInspector]
    public bool isCheatActive;

    [Header("Variables")]
    [SerializeField]
    TextMeshProUGUI counterTime;

    public bool orientToPlayer = false;

    [SerializeField]
    TextMeshProUGUI dataTime;

    public bool _isActive = true;

    private static GameController _instance;
    public static GameController Instance => _instance;
    float _timer;
    public bool isActiveMision = true;
    public int mision = 0;
    public int task = 0;
    public bool lookInputActive;
    public bool movementInputActive;
    public bool unLocked;

    [SerializeField]
    private GameObject pieceIntoChest;

    [SerializeField]
    private GameObject lockReaction;

    public bool catched;
    public bool gameOver = false;

    [Header("Parametrs")]
    public int stateToTask;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LockCursor();
        _timer = (float)timeToPlaying.minutesRequiredMision * 60f;

    }

    void Update()
    {
        if (_timer > 0)
            _timer -= Time.deltaTime;
        //Aquí hare un ternario donde en dependencia de si comienza la misión o no cambié el tiempo
        counterTime.text = _timer.ToString("000" + " seg");
        // timeToPlaying.minutesRequiredMision.ToString("00");

        dataTime.text = DateTime.Now.ToString("HH:mm:ss");

    }

    // Update is called once per frame
    public void LockCursor(bool value = false)
    {
        Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !value;
    }

    public void TogglePause(bool value)
    {
        pauseMenu.SetPause(value);
    }

    public void ToggleCheat(bool value)
    {
        isCheatActive = !value;
        cheatCanvasGroup.gameObject.SetActive(isCheatActive);
        cheatCanvasGroup.alpha = isCheatActive ? 1f : 0f;
        cheatCanvasGroup.interactable = isCheatActive ? true : false;
        cheatCanvasGroup.blocksRaycasts = isCheatActive ? true : false;
    }

    public void GameOver()
    {
        gameOver = true;
        LockCursor();
        movementInputActive = false;
        lookInputActive = false;
        endGameCanvasGroup.gameObject.SetActive(true);
        endGameCanvasGroup.alpha = 1;
        endGameCanvasGroup.interactable = true;
        endGameCanvasGroup.blocksRaycasts = true;
        Time.timeScale = 0f;
    }

    public void GameWin()
    {
        winGame.alpha = 1;
        winGame.blocksRaycasts = true;
        winGame.interactable = true;
        hudCanvas.alpha = 0;
        hudCanvas.blocksRaycasts = false;
        hudCanvas.interactable = false;
        minimapCanvas.SetActive(false);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SwitchSettingsMM.Instance.ChangeToGame();
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.performed)
            TogglePause(false);
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        _noteCanvasGroup.alpha = 0;
        _noteCanvasGroup.interactable = false;
        _noteCanvasGroup.blocksRaycasts = false;
        movementInputActive = true;
        lookInputActive = true;
        _inputController.EnableUiInputs(false);
        _inputController.EnablePlayerInputs(true);
    }

    public void OnTest(InputAction.CallbackContext context)
    {
        if (context.performed)
            Debug.Log("inputUI");

    }

    public void SwitchUIPlayer(bool value)
    {
        _inputController.EnableUiInputs(value);
        _inputController.EnablePlayerInputs(!value);
    }

    public void LockOn()
    {
        SwitchUIPlayer(false);
        unLocked = true;
        pieceIntoChest.SetActive(true);
        lockReaction.SetActive(false);
    }
}
