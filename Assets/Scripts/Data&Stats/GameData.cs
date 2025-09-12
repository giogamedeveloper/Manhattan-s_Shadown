using System;
using UnityEngine;

public class GameData : MonoBehaviour
{
    [System.Serializable]
    public struct PlayerStats
    {
        public float hpMax;
        public float damage;
        public int level;
        public int levelPlayer;
        public float exp;
        public float maxExp;
        public int stats;
        public Vector3 transform;
    }

    public int Tutorial;

    [System.Serializable]
    public struct EnemyVampire
    {
        public float Hp;
        public float damage;
    }

    [System.Serializable]
    public struct EnemyMutant
    {
        public float Hp;
        public float damage;
    }

    [System.Serializable]
    public struct EnemyRobot
    {
        public float Hp;
        public float damage;
    }

    private int _levelPlayer;
    public PlayerStats playerStats;
    public EnemyVampire enemyVampireStats;
    public EnemyMutant enemyMutantStats;
    public EnemyRobot enemyRobotStats;
    private static GameData _instance;
    public static GameData Instance => _instance;

// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            //Para que no se destruya el gameobject entre escenas.
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void InitialStats()
    {
        playerStats.level = 1;
        playerStats.hpMax = 100f;
        playerStats.damage = 20f;
        playerStats.exp = 0f;
        playerStats.maxExp = 50f;
        playerStats.stats = 0;
        playerStats.transform = transform.position;
    }

    public void UpdateStats(int level,
        Vector3 _transform,
        int i,
        int levelPlayer,
        float hpMax,
        float damage,
        float exp,
        float expMax,
        int stats)
    {
        playerStats.level = level;
        _levelPlayer = levelPlayer;
        playerStats.hpMax = hpMax;
        playerStats.damage = damage;
        playerStats.exp = exp;
        playerStats.maxExp = expMax;
        playerStats.stats = stats;
        playerStats.transform = _transform;
    }
}
