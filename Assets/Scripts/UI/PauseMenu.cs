using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionAsset actionAssets;

    [SerializeField]
    private CanvasGroup pauseCanvasGroup;

    [SerializeField]
    private GameObject mapCanvasGroup;


    bool _isActive;

    // Update is called once per frame
    public void SetPause(bool isActive)
    {
        GameController.Instance.SwitchUIPlayer(isActive);
        Time.timeScale = isActive ? 0 : 1;
        AudioController.Instance.SetMusicForScene(isActive ? "MainMenu" : "Game");
        // Actualiza el parametro de verificacion
        pauseCanvasGroup.gameObject.SetActive(isActive);
        pauseCanvasGroup.alpha = isActive ? 1 : 0;
        pauseCanvasGroup.interactable = isActive;
        pauseCanvasGroup.blocksRaycasts = isActive;
        mapCanvasGroup.gameObject.SetActive(!isActive);
    }
}
