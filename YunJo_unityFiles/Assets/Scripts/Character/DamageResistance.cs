using UnityEngine;

[System.Serializable]
public class DamageResistance
{
    public ResistanceLevel slash = ResistanceLevel.Normal;
    public ResistanceLevel pierce = ResistanceLevel.Normal;
    public ResistanceLevel blunt = ResistanceLevel.Normal;

    public float GetModifier(DamageType type)
    {
        ResistanceLevel level = ResistanceLevel.Normal;

        switch (type)
        {
            case DamageType.Slash:
                level = slash;
                break;

            case DamageType.Pierce:
                level = pierce;
                break;

            case DamageType.Blunt:
                level = blunt;
                break;
        }

        return level switch
        {
            ResistanceLevel.Fatal => 2f,
            ResistanceLevel.Weak => 1.5f,
            ResistanceLevel.Normal => 1f,
            ResistanceLevel.Endured => 0.5f,
            ResistanceLevel.Ineffective => 0.25f,
            _ => 1f
        };
    }
}