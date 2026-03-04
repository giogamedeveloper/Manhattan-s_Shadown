using System.IO;
using System.Linq;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    //Objeto que contiene toda la información de la partida
    public Data data;

    //Nombre del fichero de guardado
    public string fileName = "save.dat";

    //Combinación de ruta mas nombre de fichero
    string _dataPath;
    private static DataManager _instance;
    public static DataManager Instance => _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _dataPath = Application.persistentDataPath + "/" + fileName;
            Load();
            EnsureDataInitialized();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Load()
    {
        //Si no existe el fichero, no hacemos nada
        if (!File.Exists(_dataPath)) return;
        try
        {
            string json = File.ReadAllText(_dataPath);
            Data loadedData = JsonUtility.FromJson<Data>(json);
            if (loadedData != null)
            {
                data = loadedData;
                return;
            }
        }
        catch (Exception)
        {
            // Intentamos cargar formato binario antiguo para no perder partidas viejas.
        }

#pragma warning disable SYSLIB0011
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            using FileStream fs = File.Open(_dataPath, FileMode.Open);
            data = (Data)bf.Deserialize(fs);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"No se pudo cargar el archivo de guardado: {e.Message}");
        }
#pragma warning restore SYSLIB0011
    }

    public void Save()
    {
        EnsureDataInitialized();
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(_dataPath, json);
    }

    private void EnsureDataInitialized()
    {
        data ??= new Data();
        data.allConditions ??= Array.Empty<Condition>();
        data.allItems ??= Array.Empty<Item>();
        data.statistics ??= Array.Empty<Stat>();
        data.achievements ??= Array.Empty<Achievement>();

        if (data.inventory == null || data.inventory.Length == 0)
            data.inventory = new Item[4];

        for (int i = 0; i < data.inventory.Length; i++)
        {
            data.inventory[i] ??= new Item();
        }
    }

    /// <summary>
    /// Devuelve el estado en que se encuentra la condición recibida por parámetro
    /// </summary>
    /// <param name="conditionId"></param>
    /// <returns></returns>
    public bool CheckCondition(string IdQuest, string idSubQuest)
    {
        //Buscamos la condicion de la lista
        Condition condition = data.allConditions.SingleOrDefault(c => c.idQuest == IdQuest && c.idSubQuest == idSubQuest);
        //Si la condición no existe
        if (condition == null)
        {
            //Avisamos por consola y devolvemos false
            Debug.LogWarning("la quest con ID " + IdQuest + " no exite.");
            return false;
        }
        //De lo contrario devolvemos el valor de "isDone" de la condición
        return condition.isDone;
    }

    /// <summary>
    /// Cambia el estado de la condición indicada
    /// </summary>
    /// <param name="conditionId"></param>
    /// <param name="isDone"></param>
    public void SetCondition(string IdQuest, string idSubQuest, bool isDone)
    {
        //Buscamos la condición que cumple el criterio indicado
        Condition condition = data.allConditions.SingleOrDefault(c => c.idQuest == IdQuest && c.idSubQuest == idSubQuest);
        //Si la condición no existe
        if (condition == null)
        {
            //Avisamos por consola y devolvemos false
            Debug.LogWarning("la condicion con ID" + IdQuest + "no exite.");
            return;
        }
        //De lo contrario seteamos el valor de "isDone" de la condición
        condition.isDone = isDone;
    }

    [ContextMenu("Delete Save File")]
    public void DeleteSaveFile()
    {
        if (File.Exists(_dataPath)) File.Delete(_dataPath);
    }

    [ContextMenu("Save File")]
    public void SaveData()
    {
        Save();
    }
}
