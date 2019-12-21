using System;
using UnityEngine;

namespace BehaviorScripts
{
    public interface IBulletBehavior
    {
        //Start is called once when the bullet is created to apply some property
        //This is NOT monobehavior related.
        void Start(ProjectileScript entity);

        //Update is called every fame to apply some property to the bullet.
        //This is NOT monobehavior related.
        void Update(double dt);


        int GetDamage();

        bool GetFriendly();

        Vector3 GetInitialPosition();

        void HandleCollision(Collision2D collision);
    }
}