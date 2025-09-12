using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueTextContainer : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private TextMeshProUGUI _dialogueText;


    public void DisplayText(string text, Color textColor)
    {
        _canvasGroup.alpha = 1;
        _dialogueText.text = text;
        _dialogueText.color = textColor;
        StartCoroutine(DelayToHide(4));
    }

    IEnumerator DelayToHide(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideText();
    }

    public void HideText()
    {
        _dialogueText.text = "";
        _canvasGroup.alpha = 0;
    }
}
