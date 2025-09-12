using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// [RequireComponent(typeof(SphereCollider))]
public class Interactable : MonoBehaviour
{
    public bool triggerInteractable = false;

    [Header("Conditions")]
    public string[] conditionsQuest;

    public string[] conditionsSubQuest;

    [SerializeField]
    Transform _defaultReactions;

    [SerializeField]
    Transform _positiveReactions;

    private Queue<Reaction> _reactionQueue = new Queue<Reaction>();
    private bool _reacting = false;

    [SerializeField]
    private Transform _handle;

    [SerializeField]
    private Collider collider;

    void OnTriggerEnter(Collider other)
    {
        if (triggerInteractable && other.CompareTag("Player"))
        {
            Interact();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        collider.isTrigger = true;
    }

    public void Interact(PlayerInteract interactor = null)
    {
        if (_reacting) return;
        _reacting = true;

        if (interactor != null && _handle != null)
            interactor.StartIkAnimation(_handle);
        //Por defecto marcamos quye están cumplida todas las condiciones
        bool success = true;
        //Recorremos todasl las condiciones y las comprobamos
        foreach (string conditionIdQuest in conditionsQuest)
        {
            foreach (string conditionIdSubQuest in conditionsQuest)
            {

                //Si alguna no se cumple, cambiamos el success a false y cortamos el bucle
                if (!DataManager.Instance.CheckCondition(conditionIdQuest, conditionIdSubQuest))
                {
                    success = false;
                    break;
                }
            }
        }
        //Si se cumplen todas las condiciones y, además, el # de condiciones es mayor a 0, ejecutamos las condiciones positivas
        if (success && conditionsQuest.Length > 0)
        {
            QueueReaction(_positiveReactions);
        }
        else
        {
            QueueReaction(_defaultReactions);
        }
        NextReaction();
    }

    private void QueueReaction(Transform reactionsContainer)
    {
        //limpiamos la cola
        _reactionQueue.Clear();
        //Recorremos todas las reacciones configuradas y las ponemos en cola
        Reaction[] reactions = reactionsContainer.GetComponentsInChildren<Reaction>();
        foreach (Reaction reaction in reactions)
        {
            reaction.interactable = this;
            _reactionQueue.Enqueue(reaction);
        }
    }

    /// <summary>
    /// Incia la siguiente reacción en la cola
    /// </summary>
    public void NextReaction()
    {
        if (_reactionQueue.Count > 0)
        {
            _reactionQueue.Dequeue().ExecuteReaction();
        }
        else
        {
            _reacting = false;
        }
    }
}
