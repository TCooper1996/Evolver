using System;
using BehaviorScripts;
using UnityEngine;

namespace EntityScripts
{
    public class ModelScript : MonoBehaviour
    {
        private Entity _owner;

        private void Start()
        {
            _owner = transform.parent.GetComponent<Entity>();

            if (!_owner)
            {
                throw new Exception("Could not get Entity Script from parent.");
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            _owner.OnCollision(other);
        }
    }
}