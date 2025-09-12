using UnityEngine;

[System.Serializable]
public class Condition
{
    //Nombre de la condición. Debe ser único, ya que se usará para identificar la condición
    public string idQuest;
    public string idSubQuest;

    //Descripción de apoyo para saber de que va la condición o para que se usa.
    [TextArea]
    public string description;

    //Indica si la condición se ha cumplido
    public bool isDone;
}
