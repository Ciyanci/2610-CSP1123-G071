using UnityEngine;
public class ClashTestDriver : MonoBehaviour
{
    public CharacterUnit a;
    public CharacterUnit b;
    public Card cardA;
    public Card cardB;
    public BattleFlowController flow;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (a == null || b == null || cardA == null || cardB == null)
            {
                Debug.LogError("ClashTestDriver missing setup references!");
                return;
            }

            flow.TestClash(a, b, cardA, cardB);
        }
    }
}