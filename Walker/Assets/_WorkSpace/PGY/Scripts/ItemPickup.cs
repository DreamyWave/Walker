using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;
    public float pickupDistance = 2f;

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= pickupDistance)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Pickup();
            }
        }
    }

    void Pickup()
    {
        Inventory.instance.AddItem(item);
        Destroy(gameObject);
    }
}