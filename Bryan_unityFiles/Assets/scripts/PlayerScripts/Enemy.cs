using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 100;

    public void TakeDamage(int amount)
    {
        health -=amount;
        Debug.Log("Enemy took " + amount + "damage. HP left: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy Defeated!");
        Destroy(gameObject);
    }
}
