using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _faderCanvasGroup;

    [SerializeField]
    private float _fadeDuration;

    [SerializeField]
    //Escena inicial de carga
    private string _startingScene;

    private bool _isFading;

    private static SceneController _instance;
    public static SceneController Instance => _instance;

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
    IEnumerator Start()
    {
        if (_faderCanvasGroup != null)
            _faderCanvasGroup.alpha = 1f;
        yield return StartCoroutine(LoadSceneAndSetActive(_startingScene));
        StartCoroutine(Fade(0f));
    }

    /// <summary>
    /// Realiza un fundido visible o invisible, según el parámetro targetAlpha
    /// </summary>
    /// <param name="targetAlpha"></param>
    /// <returns></returns>
    private IEnumerator Fade(float targetAlpha)
    {
        if (_faderCanvasGroup == null)
        {
            _isFading = false;
            yield break;
        }
        //Indicamos que se está produciendo un fade
        _isFading = true;
        //Bloqueamos los raycast para evitar que el jugador interactue durante el proceso
        _faderCanvasGroup.blocksRaycasts = true;
        float timeCounter = _fadeDuration;
        float initialAlpha = _faderCanvasGroup.alpha;
        while (timeCounter > 0)
        {
            _faderCanvasGroup.alpha = Mathf.Lerp(initialAlpha, targetAlpha, 1f - (timeCounter / _fadeDuration));
            timeCounter -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        _faderCanvasGroup.alpha = targetAlpha;
        _isFading = false;
        //Si la pantalla de carga se oculta dejamos de bloquear los raycast
        if (targetAlpha == 0) _faderCanvasGroup.blocksRaycasts = false;
    }

    [ContextMenu("Fade Toggle Test")]
    public void FadeToogleTest()
    {
        if (_faderCanvasGroup.alpha == 0)
        {
            StartCoroutine(Fade(1));
        }
        else
        {
            StartCoroutine(Fade(0));
        }
    }

    /// <summary>
    /// Carga la escena de forma asíncrona
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("No se ha iniciado una escena para cargar");
            yield break;
        }
        //Para probar en el futuro
        // AsyncOperation asyncIp= SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        // asyncIp.allowSceneActivation = false;
        // yield return asyncIp;

        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        //Para forzar el actualizado de los lightprobes de la escena
        LightProbes.Tetrahedralize();
        if (AudioController.Instance != null)
            AudioController.Instance.SetMusicForScene(sceneName);
        //Recuperamos la última escena cargada
        Scene newMyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        //Marcamos la escena como activa
        SceneManager.SetActiveScene(newMyLoadedScene);
    }

    private IEnumerator FadeAndSwitchScene(string sceneName)
    {
        //Fade in
        yield return StartCoroutine(Fade(1f));
        //Inciamos descarga de escena actual
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        //Inciamos la carga de la escena indicada 
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

        //Fade out
        yield return StartCoroutine(Fade(0f));
    }

    public void FadeAndLoadScene(string sceneName)
    {
        if (!_isFading)
        {
            Debug.Log(sceneName);
            StartCoroutine(FadeAndSwitchScene(sceneName));
        }
    }
}
