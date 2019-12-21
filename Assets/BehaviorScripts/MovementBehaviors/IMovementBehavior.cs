using UnityEngine;
using UnityEngine.EventSystems;

namespace BehaviorScripts
{
    public interface IMovementBehavior
    {
        //public abstract Vector2 aim { get; set; }
        //public abstract Vector2 move { get; set; }
        
        //public abstract Vector3 position { get; }
        void SetSpeedMultiplier(float multiplier);
        void Move(float dt);

        //public abstract Vector2 velocity { get; }
        
    }
}