using System;
using ManagerScripts;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace BehaviorScripts.MovementBehaviors
{
    public class TurretMovementBehavior : MonoBehaviour
    {
        private EnemyScript _owner;
        
        [SerializeField]
        private float rotationSpeedMultiplier = 1;
        private const float BaseRotationSpeed = 2f;

        private float rotationSpeed => BaseRotationSpeed * rotationSpeedMultiplier;
        
        public Vector3 position => _owner.Position;

        private void Awake()
        {
            _owner = transform.parent.gameObject.GetComponent<EnemyScript>();
            if (!_owner)
            {
                throw new Exception("Could not get Entity Script from parent.");
            }
        }

        public void Move(float dt)
        {
            if (_owner.Target)
            {
                var relativePosition = _owner.TargetPosition;
                //Radians
                var angle = Math.Atan2(relativePosition.y, relativePosition.x);
            
                _owner.SetRotation((float)angle);
            }
            else
            {

                var relativePosition = DirectorScript.GetPlayerPosition() - position;
                var angleToTarget = (float) Math.Atan2(relativePosition.y, relativePosition.x) * Mathf.Rad2Deg - 90;
            
                var q = Quaternion.AngleAxis(angleToTarget, Vector3.forward);
                _owner.SetRotation(Quaternion.Slerp(_owner.model.transform.rotation, q, rotationSpeed * Time.deltaTime));

            }
            
        }
    }
}