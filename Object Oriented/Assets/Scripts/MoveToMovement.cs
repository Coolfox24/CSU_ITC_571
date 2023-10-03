using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MoveToMovement : Movement
{
    float moveSpeed = 1.5f;
    List<PFNode> path;

    // Start is called before the first frame update
    void Start()
    {
        path = new List<PFNode>();
        pos = new Vector3Int(0, 0);
        //Get First Path
    }

    // Update is called once per frame
    void Update()
    {
        if(path.Count == 0)
        {
            PFNode randomWalkableNode = pf.map.walkableNodes[Random.Range(0, pf.map.walkableNodes.Count)];
            //Generate new path to walk
            path = pf.map.FindPath(pos, new Vector3Int(randomWalkableNode.x, randomWalkableNode.y));
        }
        //Ensure we have a path 
        if(path.Count == 0)
        {
            return;
        }

        // Debug.Log(path[0].x + "," + path[0].y);

        //Move Towards Next Node
        float heading = Mathf.Atan2(pos.x - path[0].x, pos.y - path[0].y);
        transform.localEulerAngles = new Vector3(0, 180 + Mathf.Rad2Deg * heading, 0);

        float newXPos = transform.position.x + (moveSpeed * Time.deltaTime * (path[0].x - pos.x));
        float newYPos = transform.position.z + (moveSpeed * Time.deltaTime * (path[0].y - pos.y));
        transform.position = new Vector3(newXPos, 0, newYPos);
        // Debug.Log(transform.position.x - path[0].x);
        if(Mathf.Abs(transform.position.x - path[0].x) <= 0.1f && Mathf.Abs(transform.position.z - path[0].y ) <= 0.1f)
        {
            pos = new Vector3Int(path[0].x, path[0].y);
            path.RemoveAt(0);
        }
    }
}
