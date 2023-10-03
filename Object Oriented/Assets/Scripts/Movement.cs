using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    protected Pathfinding pf;
    protected Vector3Int pos;

    public void Setup(Pathfinding pf)
    {
        this.pf = pf;
    }
}
