using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager { get; private set;}

    public HealthPoints _playerHealth = new HealthPoints(100, 100);

    void Awake()
    {
        if (gameManager != null && gameManager != this)
        {
            Destroy(this);
        }
        else
        {
            gameManager = this;
        }
    }
}
