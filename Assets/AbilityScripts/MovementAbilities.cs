using BehaviorScripts;
using static Ability.ActionStage;

namespace AbilityScripts
{
    public static class MovementAbilities
    {
        public static void Absorb(PlayerMovement movementComponent, Ability.ActionStage stage)
        {
            switch (stage)
            {
                case Pressed:
                    //Nothing
                    break;
                
                case Held:
                    //Nothing
                    break;
                
                case Released:
                    //Nothing
                    break;
                    
            }
        }
    }
}