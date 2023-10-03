using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportMovement : Movement
{


    // Start is called before the first frame update
    void Start()
    {
        pos = new Vector3Int(0, 0); //Init to 0,0 of grid (this can be random but shouldn't matter too much)
    }

    // Update is called once per frame
    void Update()
    {
        PFNode randomWalkableNode = pf.map.walkableNodes[Random.Range(0, pf.map.walkableNodes.Count)];
        var path = pf.map.FindPath(pos, new Vector3Int(randomWalkableNode.x, randomWalkableNode.y));
        
        if(path.Count > 0)
        {
            pos.x = path[^1].x;
            pos.y = path[^1].y;
        }

        transform.position = new Vector3( pos.x, 0, pos.y);
    }
}
