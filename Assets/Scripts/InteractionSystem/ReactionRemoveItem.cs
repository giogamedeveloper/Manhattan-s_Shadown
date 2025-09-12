using UnityEngine;

public class ReactionRemoveItem : Reaction
{
    [SerializeField]
    private string _itemName;

    protected override void React()
    {
        InventoryManager.Instance.RemoveItemFromInventory(_itemName);
    }
}
