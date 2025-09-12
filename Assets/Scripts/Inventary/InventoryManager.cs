using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventorySlot[] slots;

    private static InventoryManager _instance;
    public static InventoryManager Instance => _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateInventory();
    }

    /// <summary>
    /// Actualiza visualmente los objetos que serán mostrados en la vista del inventario
    /// </summary>
    public void UpdateInventory()
    {
        Item[] inventoryItems = DataManager.Instance.data.inventory;
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            InventorySlot slot = slots[i];
            if (!string.IsNullOrWhiteSpace(inventoryItems[i].name))
            {
                slot.SetItem(inventoryItems[i]);
            }
            else
            {
                slot.Clear();
            }
        }
    }

    public bool AddItemToInventory(string itemName)
    {
        Item[] inventoryItem = DataManager.Instance.data.inventory;
        int inventoryIndex = -1;
        for (int i = 0; i < inventoryItem.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(inventoryItem[i].name))
            {
                inventoryIndex = i;
                break;
            }
        }

        if (inventoryIndex == -1) return false;

        Item item = DataManager.Instance.data.allItems.SingleOrDefault(i => i.name == itemName);
        if (item != null)
        {
            Item newItem = new Item();
            newItem.name = item.name;
            newItem.description = item.description;
            newItem.imageName = item.imageName;
            newItem._isPicked = item._isPicked;

            inventoryItem[inventoryIndex] = newItem;
            UpdateInventory();
            return true;

        }
        return false;
    }

    public void RemoveItemFromInventory(string itemName)
    {
        Item[] inventoryItem = DataManager.Instance.data.inventory;
        for (int i = 0; i < inventoryItem.Length; i++)
        {
            if (inventoryItem[i].name == itemName)
            {
                inventoryItem[i].name = "";
                UpdateInventory();
                return;
            }
        }
    }

    [ContextMenu("Test Add Item")]
    public void TestAddItemToInventory()
    {
        AddItemToInventory("key");
    }
    [ContextMenu("Test Remove Item")]
    public void TestRemoveItemToInventory()
    {
        RemoveItemFromInventory("key");
    }

    public void EmptyItem(int slotIndex)
    {
        DataManager.Instance.data.inventory[slotIndex].name = "";
        DataManager.Instance.data.inventory[slotIndex].description = "";
        DataManager.Instance.data.inventory[slotIndex].imageName = "";
        DataManager.Instance.data.inventory[slotIndex]._isPicked = false;

    }

    public void CopyItem(int fromSlotIndex, int toSlotItem)
    {
        DataManager.Instance.data.inventory[toSlotItem].name = DataManager.Instance.data.inventory[fromSlotIndex].name;
        DataManager.Instance.data.inventory[toSlotItem].description =
            DataManager.Instance.data.inventory[fromSlotIndex].description;
        DataManager.Instance.data.inventory[toSlotItem].imageName =
            DataManager.Instance.data.inventory[fromSlotIndex].imageName;
        DataManager.Instance.data.inventory[toSlotItem]._isPicked =
            DataManager.Instance.data.inventory[fromSlotIndex]._isPicked;
    }

    public void MoveItem(int fromSlotIndex, int toSlotItem)
    {
        CopyItem(fromSlotIndex, toSlotItem);
        EmptyItem(fromSlotIndex);
    }

    public void SortInventory()
    {
        //Indice del primer espacio vacío
        int firstEmpty;
        //Indicador para controlar si se ha encontrado un objeto tras un hueco vacío
        bool foundAfterEmpty = true;

        while (foundAfterEmpty)
        {
            //Para empezar ponemos el primer indice econtrado a valor -1 para indicar que no sea ha encontrado ningun hueco vacio
            firstEmpty = -1;
            //Inicializamos el bool a false para indicar que por el momento no se ha encontrado niungun elemento tras un hueco vacio
            foundAfterEmpty = false;
            //Recorremos el inventario
            Item[] inventary = DataManager.Instance.data.inventory;
            for (int i = 0; i < inventary.Length; i++)
            {
                if (string.IsNullOrEmpty(inventary[i].name) && firstEmpty < 0)
                {
                    //Si encontramos un hueco vacío, almacenamos la posición en la que se encuentra
                    firstEmpty = i;
                }
                else if (!string.IsNullOrEmpty(inventary[i].name) && firstEmpty >= 0)
                {
                    //Si encontramos un hueco con objeto y además previamente existía un hueco vacío
                    foundAfterEmpty = true;
                }
                //En caso de que haya sido encontrado un ítem trás un slot vacío 
                if (foundAfterEmpty)
                {
                    //Movemos el ojeto
                    MoveItem(i, firstEmpty);
                    //Trás realizar esta acción salimos del bucle for
                    break;

                }
            }
        }
    }
}
