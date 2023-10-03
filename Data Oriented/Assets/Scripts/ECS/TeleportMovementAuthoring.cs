using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TeleportMovementAuthoring : MonoBehaviour
{
}

public class TeleportMovementBaker : Baker<TeleportMovementAuthoring>
{
    public override void Bake(TeleportMovementAuthoring authoring)
    {
        TransformUsageFlags flags = new TransformUsageFlags();
        Entity entity = this.GetEntity(flags);
        AddComponent(entity, new TeleportMovement{
        });

    }
}
