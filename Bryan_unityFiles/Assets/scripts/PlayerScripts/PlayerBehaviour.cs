using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerTakeDmg(20);
            Debug.Log(GameManager.gameManager._playerHealth.Health);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            PlayerHeal(10);
            Debug.Log(GameManager.gameManager._playerHealth.Health);
        }*/
    }
    private void PlayerTakeDmg(int dmg)
    {
        GameManager.gameManager._playerHealth.DmgPoint(dmg);
    }
    private void PlayerHeal(int healed)
    {
        GameManager.gameManager._playerHealth.healing(healed);
    }
}
