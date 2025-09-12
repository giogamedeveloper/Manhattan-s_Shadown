using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image itemImage;

    public void SetItem(Item item)
    {
        Sprite sprite = Resources.Load<Sprite>("ItemSprites/" + item.imageName);
        itemImage.sprite = sprite;
        itemImage.gameObject.SetActive(true);
    }

    public void Clear()
    {
        itemImage.gameObject.SetActive(false);
    }
}
