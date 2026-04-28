using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;

    public int Health => currentHealth;
    public int MaxHealth => maxHealth;

    public void DmgPoint(int dmgAmount)
    {
        currentHealth -= dmgAmount;
        if (currentHealth < 0)
            currentHealth = 0;
 
            Debug.Log("Player took damage: " + dmgAmount + " HP: " + currentHealth);

    }

    public void healing(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        Debug.Log("Player healed: " + healAmount + " HP: " + currentHealth);

    }

}