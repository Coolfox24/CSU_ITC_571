using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct PathfindingParams : IComponentData
{
    public int2 startPos;
    public int2 endPos;
}
