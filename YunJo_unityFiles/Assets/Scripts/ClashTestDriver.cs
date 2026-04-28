using UnityEngine;
using System.Collections.Generic;

public class ClashTestDriver : MonoBehaviour
{
    public List<CharacterUnit> units;
    public List<Card> cards;

    public BattleFlowController flow;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (flow == null || units.Count == 0 || cards.Count == 0)
                return;

            flow.TestClash(units, cards);
        }
    }
}