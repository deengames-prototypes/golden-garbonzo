namespace Prototype.Game
{
    public static class Options
    {
        public static CombatType CombatType = CombatType.RoundByRound;
    }

    public enum CombatType
    {
        RoundByRound,
        Summary,
    }
}
