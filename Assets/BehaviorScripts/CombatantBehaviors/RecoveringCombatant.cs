using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorScripts
{
    
    public class RecoveringCombatant : CombatantBehavior
    {
        [SerializeField, RangeAttribute(0.2f, 4)]
        private float recoveryValueMultiplier = 1;

        [SerializeField, RangeAttribute(0.2f, 4)]
        private float recoveryDelayMultiplier = 1;
        
        private float m_RecoveryValue = 10f;
        private float m_TimeSinceStruck;
        private float m_RecoveryDelay = 5f;

        private new void Start()
        {
            base.Start();
            m_RecoveryValue *= recoveryValueMultiplier;
            m_RecoveryDelay *= recoveryDelayMultiplier;
        }


        public override void HandleHit(ProjectileScript bullet)
        {
            base.HandleHit(bullet);
            StopCoroutine(nameof(RecoveryTimer));
            StopCoroutine(nameof(Recover));

            if (health <= 0)
                return; 
            
            m_TimeSinceStruck = 0;
            StartCoroutine(nameof(RecoveryTimer));
        }

        private IEnumerator RecoveryTimer()
        {
            var time = 0f;
            while (time < m_RecoveryDelay)
            {
                time += Time.deltaTime;
                yield return null;
            }

            StartCoroutine(nameof(Recover));
        }


        private IEnumerator Recover()
        {
            while (healthRatio < 1)
            {
                Heal((int)m_RecoveryValue);
                DirectorScript.CreateHealPopup(m_RecoveryValue.ToString(), owner.Position);
                yield return new WaitForSeconds(1f);
            }
        }

    }
}