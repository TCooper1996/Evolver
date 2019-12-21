using BehaviorScripts;
using static Ability.ActionStage;

namespace AbilityScripts
{
    public static class CombatantAbilities
    {
        /*
        public static void Absorb(CombatantBehavior combatantComponent, Ability.ActionStage stage)
        {
            switch (stage)
            {
                case Pressed:
                    if (PlayerScript.absorbUsageRemaining == PlayerScript.absorbCooldown)
                    {
                        PlayerScript.ActivateAbsorb();
                    }
                    break;
                
                case Held:
                    //Nothing
                    break;
                
                case Released:
                    //Nothing
                    break;
            }
        }
        */

        public static void HoldFire(CombatantBehavior combatantComponent, Ability.ActionStage stage)
        {
            switch (stage)
            {
                case Pressed:
                    break;
                
                case Held:
                    //Nothing
                    break;
                
                case Released:
                    break;
            }
        }
    }
}