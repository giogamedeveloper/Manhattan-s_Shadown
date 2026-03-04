using System;
using UnityEngine;

public class PlayTimeTracker : MonoBehaviour
{
    public float minutesRequiredMision = 10f;
    public float minutesRequiredGame = 30f;
    private float elapsedTime = 0f;
    private bool achievementUnlockedMision = false;
    private bool achievementUnlockedGame = false;
    public static Action OnTimeToPlayMision;
    public static Action OnTimeToPlayGame;

    void Update()
    {
        if (achievementUnlockedMision && achievementUnlockedGame) return;

        elapsedTime += Time.deltaTime; // Suma el tiempo en segundos

        if (!achievementUnlockedMision && elapsedTime >= minutesRequiredMision * 60f) // Si alcanzamos la cantidad de minutos requerida
        {
            achievementUnlockedMision = true; // Solo se activa una vez
            OnTimeToPlayMision?.Invoke();
        }

        if (!achievementUnlockedGame && elapsedTime >= minutesRequiredGame * 60f)
        {
            achievementUnlockedGame = true;
            OnTimeToPlayGame?.Invoke();
        }
    }
}
