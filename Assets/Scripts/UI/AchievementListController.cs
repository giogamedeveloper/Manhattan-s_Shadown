using System.Linq;
using UnityEngine;

public class AchievementListController : MonoBehaviour
{
    [SerializeField]
    private AchievementRow _rowPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Achievement[] achievements = DataManager.Instance.data.achievements;
        for (int i = 0; i < achievements.Length; i++)
        {
            Stat stat = DataManager.Instance.data.statistics.Where(s => s.code == achievements[i].statCode)
                .FirstOrDefault();
            if (stat == null) continue;
            AchievementRow row = Instantiate(_rowPrefab, transform);
            row.SetAchievement(achievements[i], stat.value, i);
        }
    }
}
