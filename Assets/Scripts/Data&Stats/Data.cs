using UnityEngine;

[System.Serializable]
public class Data
{
    public string currentScene;
    public string entrancePosition;
    public Condition[] allConditions;
    public Item[] allItems;
    public Item[] inventory = new Item[4];
    public Stat[] statistics;
    public Achievement[] achievements;
}
