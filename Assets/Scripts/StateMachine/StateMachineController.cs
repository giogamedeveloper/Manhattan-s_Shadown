using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateMachineController : MonoBehaviour
{
    //Para desactivar la IA en caso de que sea necesario
    public bool isAiActive = true;

    [Space]
    // Estado actual 
    public State currentState;

    public Animator animator;
    public NavMeshAgent navMeshAgent;

    public List<Transform> wayPointsList;

    public int nextWayPoint;

    public Vector3 NextWayPoint => wayPointsList[nextWayPoint].position;


    public EnemyStats stats;

    public Transform eyes;
    public Transform target;
    public Vector3 lastSpottedPosition;

    [HideInInspector]
    public float stateTimer;

    [HideInInspector]
    public bool isTimerCounting;

    public List<Vector3> heardSounds;
    public Vector3 currentSoundPosition;


    void OnDrawGizmos()
    {
        //Para Visualizar las últimas posiciones de la IA
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(lastSpottedPosition, 0.5f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(currentSoundPosition, 0.5f);
    }


    void Start()
    {
        currentState.StartState(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAiActive) return;

        currentState.UpdateState(this);

    }

    public void TransitionToState(State nextState)
    {
        if (nextState != currentState)
        {
            currentState.EndState(this);
            currentState = nextState;
            currentState.StartState(this);
            //Reseteamoas el contador del temporizador de estado en caso de cambiar de estado
            stateTimer = 0;
            isTimerCounting = false;
            Debug.Log("Transitioning to " + currentState.name);
        }
    }

    public void HearSound(Vector3 position)
    {
        if (Vector3.Distance(transform.position, position) <= stats.hearRange)
        {
            heardSounds.Add(position);
        }
    }
}
