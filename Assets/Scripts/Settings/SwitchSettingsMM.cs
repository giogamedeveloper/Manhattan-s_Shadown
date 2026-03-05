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
    [SerializeField]
    private bool isActive;

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
            isActive = !isActive;
        settingsCanvasGroup.gameObject.SetActive(isActive);
        settingsCanvasGroup.alpha = isActive ? 1 : 0;
        settingsCanvasGroup.interactable = isActive;
        settingsCanvasGroup.blocksRaycasts = isActive;
        mainCanvasGroup.gameObject.SetActive(!isActive);
        mainCanvasGroup.interactable = !isActive;
        mainCanvasGroup.blocksRaycasts = !isActive;
        mainCanvasGroup.alpha = isActive ? 0 : 1;
        scene.SetActive(!isActive);
    }

    public void SwitchHowToPlay()
    {
        if (howToPlayCanvasGroup != null || mainCanvasGroup != null || scene != null)
            isActive = !isActive;
        howToPlayCanvasGroup.gameObject.SetActive(isActive);
        howToPlayCanvasGroup.alpha = isActive ? 1 : 0;
        howToPlayCanvasGroup.interactable = isActive;
        howToPlayCanvasGroup.blocksRaycasts = isActive;
        mainCanvasGroup.gameObject.SetActive(!isActive);
        mainCanvasGroup.interactable = !isActive;
        mainCanvasGroup.blocksRaycasts = !isActive;
        mainCanvasGroup.alpha = isActive ? 0 : 1;
        scene.SetActive(!isActive);
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
