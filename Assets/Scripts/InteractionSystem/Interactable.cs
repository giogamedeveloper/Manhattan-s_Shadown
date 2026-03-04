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
    [FormerlySerializedAs("collider")]
    private Collider _triggerCollider;

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
        if (_triggerCollider == null)
            _triggerCollider = GetComponent<Collider>();
        if (_triggerCollider == null) return;
        _triggerCollider.isTrigger = true;
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
        int conditionsCount = Mathf.Min(conditionsQuest.Length, conditionsSubQuest.Length);
        if (conditionsQuest.Length != conditionsSubQuest.Length)
        {
            Debug.LogWarning($"Interactable '{name}' tiene listas de condiciones desbalanceadas.");
            success = false;
        }

        for (int i = 0; i < conditionsCount; i++)
        {
            string conditionIdQuest = conditionsQuest[i];
            string conditionIdSubQuest = conditionsSubQuest[i];
            //Si alguna no se cumple, cambiamos el success a false y cortamos el bucle
            if (!DataManager.Instance.CheckCondition(conditionIdQuest, conditionIdSubQuest))
            {
                success = false;
                break;
            }
        }
        //Si se cumplen todas las condiciones y, además, el # de condiciones es mayor a 0, ejecutamos las condiciones positivas
        if (success && conditionsCount > 0)
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
