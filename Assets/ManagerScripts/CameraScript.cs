using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using VertexArray = System.Collections.Generic.List<UnityEngine.Vector2>;

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

