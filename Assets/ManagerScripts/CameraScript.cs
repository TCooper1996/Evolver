﻿using UnityEngine;

namespace ManagerScripts
{
    public class CameraScript : MonoBehaviour
    {
        private static CameraScript _camera;
        public static CameraScript camera => _camera;

        // Start is called before the first frame update
        private void Awake()
        {
            if (_camera)
            {
                Destroy(gameObject);
                return;
            }

            _camera = this;
        }

    }
}

