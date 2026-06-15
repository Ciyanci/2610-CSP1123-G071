//static holder
public static class BattleContext
{
    public static StageData   ActiveStage  { get; private set; }
    public static TeamRoster  ActiveRoster { get; private set; }

    public static void Set(StageData stage, TeamRoster roster)
    {
        ActiveStage  = stage;
        ActiveRoster = roster;
    }

    public static void Clear()
    {
        ActiveStage  = null;
        ActiveRoster = null;
    }
}
