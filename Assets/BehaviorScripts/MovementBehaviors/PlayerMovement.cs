using System;
using ManagerScripts;
using UnityEngine;
using Utilities;

namespace BehaviorScripts
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private PlayerScript Owner;
        
        public Vector2 move { get; set; }
        private int m_Speed = 500;
        
        public Vector2 velocity => move * m_Speed;

        public Vector2 aim { get; set; }
        


        public Vector2 GetAim()
        {
            return aim;
        }

        public void Move(float dt)
        {
            var vec = move * (m_Speed);
            Owner.AddVelocity(vec);
            
        }

    }
}