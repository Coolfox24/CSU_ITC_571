using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EntitySpawnerAuthoring : MonoBehaviour
{
    public GameObject prefab;
    public int entitiesToSpawnPerSecond;

    public bool spawnAllAtOnce = false;
    public int maxSpawnCount = 0;
}

public class EntitySpawnerBaker : Baker<EntitySpawnerAuthoring>
{
    public override void Bake(EntitySpawnerAuthoring authoring)
    {
        TransformUsageFlags flags = new TransformUsageFlags();
        Entity entity = this.GetEntity(flags);
        AddComponent(entity, new EntitySpawnerComponent{
            entityPrefab = GetEntity(authoring.prefab, flags),
            entitySpawnTimer = 1f / authoring.entitiesToSpawnPerSecond,
            spawnAllAtOnce = authoring.spawnAllAtOnce,
            maxSpawnCount = authoring.maxSpawnCount,
            hasSpawned = false
        });
    }
}