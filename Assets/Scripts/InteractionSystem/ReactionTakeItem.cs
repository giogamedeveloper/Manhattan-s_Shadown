using System.Linq;
using UnityEngine;

public class ReactionTakeItem : Reaction
{
    [SerializeField]
    private InteractableItem _currentItem;

    private void PickUpItem()
    {
        Item result = DataManager.Instance.data.allItems.SingleOrDefault(i => i.name == _currentItem.itemName);
        if (result == null)
        {
            Debug.LogWarning("El item con nombre " + _currentItem.itemName + "no existe");
            return;
        }
        if (InventoryManager.Instance.AddItemToInventory(_currentItem.itemName))
        {
            result._isPicked = true;
            _currentItem.gameObject.SetActive(false);
        }
    }

    protected override void React()
    {
        PickUpItem();
    }
}
