using System;
using UnityEngine;

public class AchievementUI : MonoBehaviour
{
    public AchievementPanel panelPrefab;
    public float timeToRemove = 3f;

    void OnEnable()
    {
        AchievementManager.OnAchievementUnlock += SetAndShow;
    }

    void OnDisable()
    {
        AchievementManager.OnAchievementUnlock -= SetAndShow;
    }

    public void SetAndShow(string name, string imageName)
    {
        AchievementPanel panel = Instantiate(panelPrefab, transform);
        Sprite iconSprite = Resources.Load<Sprite>("Achievements Icons/" + imageName);
        panel.SetAchievement(name, iconSprite);
        Destroy(panel.gameObject, timeToRemove);
    }
}
