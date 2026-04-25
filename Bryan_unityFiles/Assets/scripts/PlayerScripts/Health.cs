using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    
    int current_health;
    int current_maxhealth;
    public int Health
    {
        get
        {
            return current_health;
            
        }
        set
        {
            current_health = value;    
        }
    }

    public int MaxHealth
    {
        get
        {
            return current_maxhealth;
            
        }
        set
        {
            current_maxhealth = value;    
        }
    }
    public PlayerHealth(int health, int maxHealth)
    {
        current_health = health;
        current_maxhealth = maxHealth;
    }
    public void DmgPoint (int dmgAmount)
    {
        if (current_health > 0)
        {
            current_health -= dmgAmount;
        }
    }

    public void healing (int healAmount)
    {
         if (current_health < current_maxhealth)
        {
            current_health += healAmount;
        }
        if (current_health > current_maxhealth)
        {
            current_health = current_maxhealth;
        }
    }
}