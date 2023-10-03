using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct EntitySpawnerComponent : IComponentData
{
    public Entity entityPrefab;

    public bool spawnAllAtOnce;

    public float timeSinceLastSpawn;
    public float entitySpawnTimer;

    public int maxSpawnCount;
    public bool hasSpawned;
}
