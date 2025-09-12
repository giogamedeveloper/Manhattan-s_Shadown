[System.Serializable]
public class Achievement
{
    //Nombre del logro 
    public string name;

    //Estadistica a verificar
    public string statCode;

    //Nombre del Sprite para el logro
    public string imageName;
    public string imageNameBase;


    //Descripcion del logro
    public string description;

    //Cantidad a conseguir para obtener el logro
    public int targetAmount;

    //Bool para controlar si el logro ya ha sido desbloqueado
    public bool unlocked = false;
}
