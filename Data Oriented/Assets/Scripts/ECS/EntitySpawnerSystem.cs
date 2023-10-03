using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


public partial class EntitySpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        SystemAPI.TryGetSingletonRW<EntitySpawnerComponent>(out RefRW<EntitySpawnerComponent> esc);
        if(!esc.IsValid)
        {
            return;
        }
        
        esc.ValueRW.timeSinceLastSpawn -= SystemAPI.Time.DeltaTime;
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(World.Unmanaged);

        if(!esc.ValueRW.spawnAllAtOnce)
        {
            if(esc.ValueRW.timeSinceLastSpawn < 0)
            {
                //Spawn

                ecb.Instantiate(esc.ValueRW.entityPrefab);

                //Reset
                esc.ValueRW.timeSinceLastSpawn = esc.ValueRW.entitySpawnTimer;
            }
        }
        else
        {
            if(esc.ValueRW.hasSpawned)
            {
                return;
            }

            //Spawn all the entities at one time
            for(int i = 0; i < esc.ValueRW.maxSpawnCount; i++)
            { 
                ecb.Instantiate(esc.ValueRW.entityPrefab);
            }

            //Turn off more spawning
            esc.ValueRW.hasSpawned = true;
        }
        // EntityQuery ppQuery = EntityManager.CreateEntityQuery(typeof(PathfindingParams));

        // ecb.RemoveComponent<PathfindingParams>(ppQuery, EntityQueryCaptureMode.AtRecord);
    }
}
