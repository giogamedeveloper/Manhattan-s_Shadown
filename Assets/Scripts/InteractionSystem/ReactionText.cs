using UnityEngine;

public class ReactionText : Reaction
{
    [SerializeField]
    private DialogueTextContainer _dialogueText;

    [SerializeField]
    string _text;

    [SerializeField] Color _color;

    protected override void React()
    {
        TakeText();
        _dialogueText.DisplayText(_text, _color);
    }

    private void TakeText()
    {
        _text = TranslateManager.Instance.GetText(_text);
    }
}
