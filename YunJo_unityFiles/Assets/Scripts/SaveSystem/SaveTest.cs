using UnityEngine;

public class SaveTest : MonoBehaviour
{
    [Header("Drag a CardData here")]
    public CardData testCard;

    void Update()
    {
        // Press S to unlock and save the card
        if (Input.GetKeyDown(KeyCode.S))
        {
            GameManager.Instance.UnlockCard(testCard);
            Debug.Log("[TEST] Card unlocked and saved.");
        }

        // Press L to print loaded inventory
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log($"[TEST] Cards in inventory: {CardInventory.GetAll().Count}");

            foreach (CardData card in CardInventory.GetAll())
            {
                Debug.Log($"- {card.Name}");
            }
        }

        // Press Delete to erase the save file
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            GameManager.Instance.DeleteSave();
            Debug.Log("[TEST] Save deleted.");
        }
    }
}