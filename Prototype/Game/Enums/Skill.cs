using System.ComponentModel;

namespace Prototype.Game.Enums
{
    enum Skill
    {
        [Description("Heals to full health")]
        Heal,

        [Description("Increases defense for three rounds of combat")]
        StoneSkin,
        
        [Description("Creates a shield that soaks up 20 points of damage")]
        PhaseShield,

        [Description("Hits for extra damage and starts combat")]
        Kick,

        [Description("Increases the number of attacks for one battle")]
        Focus,

        [Description("Releases a swarm that damages all monsters in the current room")]
        NanoSwarm,
    }
}
