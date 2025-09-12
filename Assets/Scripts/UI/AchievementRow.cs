using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementRow : MonoBehaviour
{
    [SerializeField]
    private Image _iconImage;

    [SerializeField]
    private Image _progressImageBase;

    [SerializeField]
    private Image _progressImageFront;

    [SerializeField]
    private TextMeshProUGUI _titleText;

    [SerializeField]
    private TextMeshProUGUI _descriptionText;


    public void SetAchievement(Achievement achievement, int currentAmount, int value)
    {
        _iconImage.sprite = Resources.Load<Sprite>("Achievements Icons/" + achievement.imageName);
        _titleText.text = TranslateManager.Instance.GetText(achievement.name);
        _descriptionText.text = TranslateManager.Instance.GetText(achievement.description);
        _progressImageBase.sprite = Resources.Load<Sprite>("Achievements Icons/" + achievement.imageName);
        _progressImageFront.sprite = Resources.Load<Sprite>("Achievements Icons/" + achievement.imageNameBase);
        float progress = (float)currentAmount / (float)achievement.targetAmount;
        _progressImageBase.fillAmount = progress;
    }
}
