public static class SlotUIUpdater
{
    public static void Refresh(CharacterUnit unit)
    {
        if (unit.slotRowUI != null)
            unit.slotRowUI.Refresh();
    }
}