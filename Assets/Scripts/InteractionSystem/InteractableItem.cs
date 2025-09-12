using System.Linq;
using UnityEngine;

public class InteractableItem : Interactable
{
    public string itemName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        if (IsPicked())
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Devuelve true si el objeto ya ha sido recogido
    /// </summary>
    /// <returns></returns>
    protected bool IsPicked()
    {
        Item item = DataManager.Instance.data.allItems.SingleOrDefault(i => i.name == itemName);
        if (item == null)
        {
            Debug.LogWarning("No existe el item con el nombre" + itemName);
            return false;
        }
        return item._isPicked;
    }
}
