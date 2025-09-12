using System;
using UnityEngine;

public class ChangeDirectionCollider : Reaction
{
    [SerializeField]
    private DialogueTextContainer _dialogueText;

    [SerializeField]
    string _text;

    [SerializeField] Color _color;
    [SerializeField]
    private Collider _collider;


    void OnCollisionEnter(Collision other)
    {
        Debug.Log(other.collider.name);
    }

    void OnTriggerEnter(Collider other)
    {
        _collider.isTrigger = false;
        React();
    }

    protected override void React()
    {
        _dialogueText.DisplayText(_text, _color);
    }
}
