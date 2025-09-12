using System.Linq;
using UnityEngine;

public class Cheats : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    StateMachineController stateMachineController;

    [SerializeField]
    State noDetectionState;

    [SerializeField]
    State originalState;

    [SerializeField] GameObject hint;

    [SerializeField]
    private InteractableItem[] _allItem;

    public void NoDetection(bool detection)
    {
        Debug.Log(detection);
        stateMachineController.TransitionToState(detection ? noDetectionState : originalState);
    }

    public void ShowCodeChest(bool active)
    {
        hint.SetActive(active ? true : false);
    }

    public void AllItems(bool toggle)
    {
        if (toggle)
        {
            AddItems();
        }
        else
        {
            RemoveItems();
        }

    }

    public void AddItems()
    {
        foreach (InteractableItem itemName in _allItem)
        {
            // Buscar el item en la data
            Item result = DataManager.Instance.data.allItems.SingleOrDefault(i => i.name == itemName.itemName);
            if (result == null)
            {
                Debug.LogWarning("El item con nombre " + itemName + " no existe");
                continue; // pasa al siguiente item
            }

            // Intentar agregar el item al inventario
            if (InventoryManager.Instance.AddItemToInventory(itemName.itemName))
            {
                result._isPicked = true;
            }
            else
            {
                Debug.LogWarning("No se pudo agregar el item " + itemName);
            }
        }
    }

    public void RemoveItems()
    {
        foreach (InteractableItem current in _allItem)
        {

            Item result = DataManager.Instance.data.inventory.SingleOrDefault(i => i.name == current.itemName);
            if (result == null)
            {
                Debug.LogWarning("El item con nombre " + current.itemName + "no existe");
                return;
            }
            InventoryManager.Instance.RemoveItemFromInventory(current.itemName);
        }
    }
}
