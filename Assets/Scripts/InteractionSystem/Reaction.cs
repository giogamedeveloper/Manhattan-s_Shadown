using System.Collections;
using UnityEngine;

public class Reaction : MonoBehaviour
{
    //Descripción de la reacción, como una nota para el editor, usando TextArea para mostrar más información en el editor
    [TextArea]
    public string description;

    //Tiempo de espera para ejecutar la reacción 
    [SerializeField]
    float _delay;

    float _delayCounter;

    [HideInInspector]
    public Interactable interactable;

    protected virtual void React()
    {
        //Aquí va el código de la reacción
    }

    protected virtual void PostReact()
    {
        //Una vez terminada la reacción, le decimos al interactrable que puede llamar a la siguiente.
        interactable.NextReaction();
    }

    protected virtual IEnumerator DelayReact()
    {
        _delayCounter = _delay;
        while (_delayCounter > 0)
        {
            yield return new WaitForEndOfFrame();
            _delayCounter -= Time.deltaTime;
        }
        React();
        PostReact();
    }

    public virtual void ExecuteReaction()
    {
        StartCoroutine(DelayReact());
    }
}
