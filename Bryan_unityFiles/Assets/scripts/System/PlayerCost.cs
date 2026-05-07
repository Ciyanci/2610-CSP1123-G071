using UnityEngine;

public class PlayerCost : MonoBehaviour
{
    [SerializeField] private int maxEnergy = 10;

    public int CurrentEnergy { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        CurrentEnergy = maxEnergy;
    }

    public void StartTurn()
    {
        CurrentEnergy = maxEnergy;
    }

    public bool CanSpend(int cost)
    {
        return CurrentEnergy >= cost;
    }

    public bool Spend(int cost)
    {
        if (!CanSpend(cost)) return false;
        CurrentEnergy -= cost;
        return true;
    }

}
