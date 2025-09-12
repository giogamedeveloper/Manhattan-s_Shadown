using System.Linq;
using UnityEngine;

public class ReactionPutItem : Reaction
{
    [SerializeField]
    private InteractableItem[] _currentItem;

    int offset;

    protected override void React()
    {
        LetItem();
    }

    void LetItem()
    {
        foreach (InteractableItem current in _currentItem)
        {

            Item result = DataManager.Instance.data.inventory.SingleOrDefault(i => i.name == current.itemName);
            if (result == null)
            {
                Debug.LogWarning("El item con nombre " + current.itemName + "no existe");
                return;
            }
           
            Vector3 position = gameObject.transform.position + new Vector3(offset, 0, 0);
            Transform item = Instantiate(current.transform, position, Quaternion.identity);
            InventoryManager.Instance.RemoveItemFromInventory(current.itemName);
        }
    }
}
