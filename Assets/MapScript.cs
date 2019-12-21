using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class MapScript : MonoBehaviour
{
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private GameObject connectorPrefab;
    [SerializeField] private GameObject chestPrefab;
    [SerializeField] private GameObject openedChestPrefab;

    public static GameObject CurrentRoom
    {
        get;
        private set;
    }

    private Dictionary<Vector3, GameObject> connectors;

    private int roomSize = 150;

    public void UpdateMap()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        
        connectors = new Dictionary<Vector3, GameObject>();
        
        foreach (var room in World.GetWorld().Values)
        {
            if (!room.visited)
                continue;

            
            var pos = new Vector3(room.position.Item1*roomSize - (4*roomSize), room.position.Item2*roomSize - (4*roomSize), 0);
            
            GameObject g;
            
            if (World.heavyScenes.Contains(room.SceneIndex))
            {
                if (DirectorScript.IsMacGuffinObtained)
                    g = Instantiate(openedChestPrefab, transform);
                else
                {
                    g = Instantiate(chestPrefab, transform);
                }
                
            }
            else
            {
                g = Instantiate(roomPrefab, transform);
            }

            g.transform.localPosition = pos;
            
            if (room.position.Equals(DirectorScript.CurrentRoom))
            {
                CurrentRoom = g;
            }
            
            foreach (DirectorScript.Direction direction in Enum.GetValues(typeof(DirectorScript.Direction)))
            {
                if (!room.HasNeighbor(direction))
                    continue;

                if (connectors.ContainsKey(pos))
                    continue;
                
                var c = Instantiate(connectorPrefab, g.transform);
                c.transform.RotateAround(g.transform.position, Vector3.forward, (int)direction * 90);
                connectors[c.transform.position] = c;

            }
            
            
        }
    }

}
