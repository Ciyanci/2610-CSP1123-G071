// ClashOddsCalculator.cs
using UnityEngine;

public static class ClashOddsCalculator
{
    // Returns player win probability 0..1 given two dice ranges + powers
    public static float WinProbability(
        int minA, int maxA, int powerA,
        int minB, int maxB, int powerB)
    {
        int wins = 0, losses = 0, draws = 0;
        int total = 0;

        for (int a = minA; a <= maxA; a++)
        {
            for (int b = minB; b <= maxB; b++)
            {
                int scoreA = a + powerA;
                int scoreB = b + powerB;

                if      (scoreA > scoreB) wins++;
                else if (scoreB > scoreA) losses++;
                else                      draws++;

                total++;
            }
        }

        if (total == 0) return 0.5f;

        // Draws split evenly
        return (wins + draws * 0.5f) / total;
    }

    // Returns a guidance string for the info bar
    public static string OddsLabel(float winChance)
    {
        if      (winChance >= 0.70f) return "Favoured to Win";
        else if (winChance >= 0.55f) return "Slight Advantage";
        else if (winChance >= 0.45f) return "Even Odds";
        else if (winChance >= 0.30f) return "Slight Disadvantage";
        else                         return "Likely to Lose";
    }

    public static Color OddsColor(float winChance)
    {
        if      (winChance >= 0.70f) return new Color(0.2f, 0.9f, 0.3f);
        else if (winChance >= 0.55f) return new Color(0.6f, 0.9f, 0.2f);
        else if (winChance >= 0.45f) return Color.white;
        else if (winChance >= 0.30f) return new Color(0.9f, 0.6f, 0.2f);
        else                         return new Color(0.9f, 0.2f, 0.2f);
    }
}
