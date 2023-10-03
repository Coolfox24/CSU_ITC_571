using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections.LowLevel.Unsafe;

[BurstCompile]
public partial struct TeleportISystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        JobHandle handle = new Pathfinding.FindPathJob
        {
        }.ScheduleParallel(state.Dependency);   

        handle.Complete();

        JobHandle handle2 =  new TeleportJob
        {

        }.ScheduleParallel(state.Dependency);   

        handle2.Complete();

        SystemAPI.TryGetSingletonRW<RandomComponent>(out RefRW<RandomComponent> random);

        if(!random.IsValid)
        {
            return;
        }

        //Generate new locations for the objects
        new RandomNodeJob{
            random = random
        }.Run();

    }

    [BurstCompile]
    private partial struct TeleportJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(ref DynamicBuffer<PathPosition> pathPos, RefRW<LocalTransform> localTransform, TeleportMovement teleportMovement, RefRW<PathfindingParams> pp, RefRW<PathIndexComponent> pathIndex)
        {
            if(pathPos.Length == 0)
            {
                return;
            }

            localTransform.ValueRW.Position = new float3(pathPos[0].position.x, 0, pathPos[0].position.y);
            pp.ValueRW.startPos = pp.ValueRW.endPos;
            pathIndex.ValueRW.pathIndex = -1;
        }
    }

    [BurstCompile]
    private partial struct RandomNodeJob : IJobEntity
    {
        [NativeDisableUnsafePtrRestriction]
        public RefRW<RandomComponent> random;

        [BurstCompile]
        public void Execute(RefRW<PathfindingParams> pp)
        {
            pp.ValueRW.endPos = new int2(random.ValueRW.random.NextInt(0, 20), random.ValueRW.random.NextInt(0, 20));
        }
    }
} 