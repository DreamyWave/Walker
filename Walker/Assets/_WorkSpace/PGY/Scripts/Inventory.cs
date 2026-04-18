using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public List<Item> items = new List<Item>();

    public int maxSlots = 5;

    public int selectedSlot = 0;



    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            selectedSlot++;
        }

        if (scroll < 0f)
        {
            selectedSlot--;
        }

        if (selectedSlot >= maxSlots)
            selectedSlot = 0;

        if (selectedSlot < 0)
            selectedSlot = maxSlots - 1;

        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedSlot = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) selectedSlot = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) selectedSlot = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4)) selectedSlot = 3;
        if (Input.GetKeyDown(KeyCode.Alpha5)) selectedSlot = 4;
        

        if (Input.GetMouseButtonDown(0))
        {
            UseItem();
        }
    }

    void UseItem()
    {
        if (items.Count == 0) return;

        Item item = items[selectedSlot];

        FindObjectOfType<PlayerStatus>().Heal(item.healAmount);

        items.RemoveAt(selectedSlot);
    }

    private void Awake()
    {
        instance = this;
    }

    public void AddItem(Item item)
    {
        if (items.Count >= maxSlots)
        {
            Debug.Log("¿Œ∫•≈‰∏Æ ∞°µÊ¬¸");
            return;
        }

        items.Add(item);
        Debug.Log(item.itemName + " »πµÊ");
    }
}