using Assets.WUG.Scripts;
using UnityEngine;

public class ItemGO : MonoBehaviour
{
    public ItemDetails ItemDetails;
    private void OnCollisionEnter2D(Collision2D collision)
    {
       
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CharacterLogicController characterLogicController = collision.gameObject.GetComponent<CharacterLogicController>();
        if (characterLogicController != null)
        {
            InventoryController inventoryController = characterLogicController.GetInventoryController();
            if (inventoryController.HaveSpace(ItemDetails))
            {
                inventoryController.AddItem(ItemDetails);
                OnPickup();
            }
        }
    }
    private void OnPickup()
    {
        Destroy(gameObject);
    }
}