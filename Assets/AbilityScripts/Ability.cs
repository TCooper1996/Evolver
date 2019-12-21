using System.Collections.Generic;
using AbilityScripts;
using AbilityTuple = 
    System.Tuple<System.Action<BehaviorScripts.PlayerMovement, Ability.ActionStage>, 
    System.Action<BehaviorScripts.CombatantBehavior, Ability.ActionStage>, 
    System.Action<BehaviorScripts.ShooterBehavior, Ability.ActionStage>>;

public class Ability
{
    public enum ActionStage
    {
        Pressed,
        Held,
        Released,
        Untouched
    }

    public enum AbilityName
    {
        Guard,
    }

    public AbilityTuple ability { get; set; }
    public ActionStage stage { get; set; }
    
    /*
    private static readonly Dictionary<AbilityName, AbilityTuple> AbilitySets = new Dictionary<AbilityName, AbilityTuple>(){
    {
        AbilityName.Guard, new AbilityTuple(MovementAbilities.Absorb, CombatantAbilities.Absorb, null)
    }};
    */

    public Ability()
    {
        ability = null;
        stage = ActionStage.Untouched;
    }

    /*
    public static AbilityTuple GetAbilitySet(AbilityName name)
    {
        return AbilitySets[name];
    }
    */

    
}
