using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PathIndexAuthoring : MonoBehaviour
{
}

public class PathIndexBaker : Baker<PathIndexAuthoring>
{
    public override void Bake(PathIndexAuthoring authoring)
    {
        TransformUsageFlags flags = new TransformUsageFlags();
        Entity entity = this.GetEntity(flags);
        AddComponent(entity, new PathIndexComponent{
            pathIndex = -1
        });
                
        //Add our path buffer to the entity
        AddBuffer<PathPosition>(entity);

    }
}
