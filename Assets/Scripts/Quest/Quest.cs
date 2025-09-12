using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Objects/Quest", order = 1)]
public partial class Quest : ScriptableObject
{
    [System.Serializable]
    public struct Mision
    {
        public string name;
        public string description;
        public QuestType type;


        [System.Serializable]
        public enum QuestType
        {
            Gathering,
            Kill,
            Arrive
        }

        [Header("Gathering Mision")]
        public bool isDifferentItems;

        public bool isTakeItems;
        public List<ItemsToGather> data;

        [System.Serializable]
        public struct ItemsToGather
        {
            public string name;
            public int cant;
            public int itemID;
        }

        [Header("Kill Mision")]
        public int cant;

        public int enemyID;

        [Header("Rewards")]
        public int gold;

        public int exp;
    }

    public Mision[] misions;
}
