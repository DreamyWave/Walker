using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Image[] slots;

    void Update()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < Inventory.instance.items.Count)
            {
                slots[i].sprite = Inventory.instance.items[i].icon;
                slots[i].enabled = true;
            }
            else
            {
                slots[i].enabled = false;
            }
        }
    }
}