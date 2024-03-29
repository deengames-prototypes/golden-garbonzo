﻿namespace Prototype.Game
{
    public static class Options
    {
        public static CombatType CombatType = CombatType.RoundByRound;
        public static SpeechMode SpeechMode = SpeechMode.Detailed;
    }

    public enum CombatType
    {
        RoundByRound,
        Summary,
    }

    public enum SpeechMode
    {
        Detailed,
        Summary
    }
}
