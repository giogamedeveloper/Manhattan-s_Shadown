using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class AchievementPanel : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI title;

    public void SetAchievement(string titleText, Sprite iconSprite)
    {
        icon.sprite = iconSprite;
        title.text = titleText;
    }
}
