public enum SlotState
{
    Empty,      // nothing assigned
    Planned,    // card + target set (preview phase)
    Committed,  // locked in for combat resolution
    Executed    // already resolved in combat
}