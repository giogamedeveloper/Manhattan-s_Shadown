using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    private Interactable _currentInteractable;

    [SerializeField]
    private Image _interactionIndicator;

    [SerializeField]
    private Image _interactionDialogue;

    [SerializeField]
    private Image _interactionPut;

    [SerializeField]
    Transform _head;

    [SerializeField]
    private LayerMask _interactableLayerMask;

    [SerializeField]
    private float _range;

    [SerializeField]
    ChainIKConstraint _rHandIK;

    public float ikAnimationTime = 1f;
    public AnimationCurve ikAnimationCurve;
    private float _ikCoroutineTimer;

    private Coroutine _ikAnimationCoroutine;

    [SerializeField]
    private Transform _rHandIKTarget;

    void FixedUpdate()
    {
        InteractLock();
    }

    private void InteractLock()
    {
        RaycastHit hit;
        if (Physics.Raycast(_head.position, _head.forward, out hit, _range, _interactableLayerMask) &&
            hit.collider.TryGetComponent(out Interactable interactable))
        {
            _currentInteractable = interactable;
            // Primero, desactiva todos los indicadores
            _interactionIndicator.gameObject.SetActive(false);
            _interactionDialogue.gameObject.SetActive(false);
            _interactionPut.gameObject.SetActive(false);

            // Activa solo el indicador correspondiente al layer del objeto
            int layer = hit.collider.gameObject.layer;
            if (layer == LayerMask.NameToLayer("Interactable"))
            {
                _interactionIndicator.gameObject.SetActive(true);
            }
            else if (layer == LayerMask.NameToLayer("Put"))
            {
                _interactionPut.gameObject.SetActive(true);
            }
            else if (layer == LayerMask.NameToLayer("Dialogue"))
            {
                _interactionDialogue.gameObject.SetActive(true);
            }
        }
        else if (_currentInteractable != null)
        {
            _currentInteractable = null;
            _interactionIndicator.gameObject.SetActive(false);
            _interactionDialogue.gameObject.SetActive(false);
            _interactionPut.gameObject.SetActive(false);
        }
    }

    public void Interact()
    {
        if (_currentInteractable == null) return;
        _currentInteractable.Interact(this);
    }

    public void StartIkAnimation(Transform ikTarget)
    {
        _rHandIK.data.target.SetParent(ikTarget);
        _rHandIKTarget.localPosition = Vector3.zero;
        _rHandIKTarget.rotation = Quaternion.Euler(Vector3.zero);
        if (_ikAnimationCoroutine != null)
        {
            StopCoroutine(_ikAnimationCoroutine);
        }
        _ikAnimationCoroutine = StartCoroutine(IKWeightLerp());
    }

    private IEnumerator IKWeightLerp()
    {
        _ikCoroutineTimer = 0f;
        while (_ikCoroutineTimer < ikAnimationTime)
        {
            _rHandIK.weight = ikAnimationCurve.Evaluate(_ikCoroutineTimer / ikAnimationTime);
            _ikCoroutineTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
