using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "State Machine/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    //Velocidades
    public float patrolSpeed = .5f;
    public float attackSpeed = 2f;

    // Alcance para detenccion de objetivos
    public float reach = 50f;

    //Tiempo que tarda en abandonar un enemigo
    public float timeToDisengage = 20f;

    //Rango de ataque mínimo
    public float minAttackRange = 2f;

    //Tamaño de la esfera proyectada para determinar si ve al jugador
    public float lookSphereCastRadius = .8f;

    //Ángulo de visión del enemigo
    public float fieldOfView = 90;

    //Layer de los objetivos
    public LayerMask targetLayers;
    public float hearRange = 5f;
}
