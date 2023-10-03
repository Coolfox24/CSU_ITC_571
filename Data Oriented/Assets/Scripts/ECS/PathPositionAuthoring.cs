using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PathPositionAuthoring : MonoBehaviour
{
}


public class PathPositionBaker : Baker<PathPositionAuthoring>
{
    public override void Bake(PathPositionAuthoring authoring)
    {
        TransformUsageFlags flags = new TransformUsageFlags();
        Entity entity = this.GetEntity(flags);
        AddComponent(entity, new PathIndexComponent{        
        });
                
        //Add our path buffer to the entity
        AddBuffer<PathPosition>(entity);
    }
}
