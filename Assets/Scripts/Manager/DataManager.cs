using System.IO;
using System.Linq;
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
        }
        else
        {
            Destroy(this);
        }
    }

    private void Load()
    {
        //Si no existe el fichero, no hacemos nada
        if (!File.Exists(_dataPath)) return;
        //Objeto utilizado para serializar/deserializar en binario
        BinaryFormatter bf = new BinaryFormatter();
        //abrimos el fichero para lectura
        FileStream fs = File.Open(_dataPath, FileMode.Open);
        //Deserializamos y volcamos la indo a nuestro objeto data
        data = (Data)bf.Deserialize(fs);
        //Una vez completada la operacion, cerramos el fichero
        fs.Close();
    }

    public void Save()
    {
        //Objeto utilizado para serializar/deserializar en binario
        BinaryFormatter bf = new BinaryFormatter();
        //Creamos el fichero de guardado
        FileStream fs = File.Create(_dataPath);
        //Serializamos el objeto data
        bf.Serialize(fs, data);
        //Una vez completada la operacion, cerramos el fichero
        fs.Close();
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
