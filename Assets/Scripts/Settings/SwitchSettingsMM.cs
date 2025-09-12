using System;
using UnityEngine;

public class SwitchSettingsMM : MonoBehaviour
{
    [SerializeField]
    CanvasGroup mainCanvasGroup;

    [SerializeField]
    CanvasGroup settingsCanvasGroup;

    [SerializeField]
    CanvasGroup howToPlayCanvasGroup;

    [SerializeField]
    GameObject scene;

    private static SwitchSettingsMM _instance;
    public static SwitchSettingsMM Instance => _instance;
    public bool _changeLenguage;
    [SerializeField]
    private bool _isActive;

    void Awake()
    {
        // TranslateManager.Instance.defaultLanguage = PlayerPrefs.GetString("idioma");
        if (_instance == null)
        {
            _instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SwitchSettings()
    {
        if (settingsCanvasGroup != null || mainCanvasGroup != null || scene != null)
            _isActive = !_isActive;
        settingsCanvasGroup.gameObject.SetActive(_isActive);
        settingsCanvasGroup.alpha = _isActive ? 1 : 0;
        settingsCanvasGroup.interactable = _isActive;
        settingsCanvasGroup.blocksRaycasts = _isActive;
        mainCanvasGroup.gameObject.SetActive(!_isActive);
        mainCanvasGroup.interactable = !_isActive;
        mainCanvasGroup.blocksRaycasts = !_isActive;
        mainCanvasGroup.alpha = _isActive ? 0 : 1;
        scene.SetActive(!_isActive);
    }

    public void SwitchHowToPlay()
    {
        if (howToPlayCanvasGroup != null || mainCanvasGroup != null || scene != null)
            _isActive = !_isActive;
        howToPlayCanvasGroup.gameObject.SetActive(_isActive);
        howToPlayCanvasGroup.alpha = _isActive ? 1 : 0;
        howToPlayCanvasGroup.interactable = _isActive;
        howToPlayCanvasGroup.blocksRaycasts = _isActive;
        mainCanvasGroup.gameObject.SetActive(!_isActive);
        mainCanvasGroup.interactable = !_isActive;
        mainCanvasGroup.blocksRaycasts = !_isActive;
        mainCanvasGroup.alpha = _isActive ? 0 : 1;
        scene.SetActive(!_isActive);
    }

    public void ChangeToGame()
    {
        SceneController.Instance.FadeAndLoadScene("Game");
    }

    public void ChangeToAchievements()
    {
        SceneController.Instance.FadeAndLoadScene("Achievements");
    }

    public void ChangeToMainMenu()
    {
        SceneController.Instance.FadeAndLoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
