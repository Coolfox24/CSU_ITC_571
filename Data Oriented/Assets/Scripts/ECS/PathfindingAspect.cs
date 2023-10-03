using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public readonly partial struct PathfindingAspect : IAspect
{
    public readonly Entity entity;
    public readonly RefRO<PathfindingParams> pfParam;

    public readonly RefRW<PathIndexComponent> pathIndex;
}
