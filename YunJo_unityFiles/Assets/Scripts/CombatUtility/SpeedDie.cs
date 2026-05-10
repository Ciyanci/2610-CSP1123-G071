using UnityEngine;

[System.Serializable]
public class SpeedDie
{
    public int value;

    public bool used;

    public Card assignedCard;

    public CharacterUnit target;
    public SpeedDie targetDie;

    public SpeedDiceUI ui;
    
    public void Roll()
    {
        value = Random.Range(1, 10);
        used = false;

        if (ui != null)
        {
            ui.SetValue(value);
            ui.Show();
        }
    }

    public void Clear()
    {
        assignedCard = null;
        target = null;
        targetDie = null;
        used = false;
    }
}