using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PathfindingParamsAuthoring : MonoBehaviour
{
    public int2 startPos;
    public int2 endPos;
}


public class PathfindingParamsAuthoringBaker : Baker<PathfindingParamsAuthoring>
{
    public override void Bake(PathfindingParamsAuthoring authoring)
    {
        TransformUsageFlags flags = new TransformUsageFlags();
        Entity entity = this.GetEntity(flags);
        AddComponent(entity, new PathfindingParams{
            startPos = authoring.startPos,
            endPos = authoring.endPos
        });
    }
}