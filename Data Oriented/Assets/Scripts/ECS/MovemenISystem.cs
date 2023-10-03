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
public partial struct MovementISystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        JobHandle handle = new Pathfinding.FindPathJob
        {
        }.ScheduleParallel(state.Dependency);   

        handle.Complete();

        JobHandle handle2 =  new MovementJob
        {
            deltaTime = Time.deltaTime
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
    private partial struct MovementJob : IJobEntity
    {
        public float deltaTime;

        [BurstCompile]
        public void Execute(ref DynamicBuffer<PathPosition> pathPos, RefRW<LocalTransform> localTransform, MovementComponent movement, RefRW<PathfindingParams> pp, RefRW<PathIndexComponent> pathIndex)
        {
            if(pathPos.Length == 0)
            {
                return;
            }
            int2 nextPathPos = pathPos[pathIndex.ValueRW.pathIndex].position;

            float3 targetPos = new float3(nextPathPos.x, 0, nextPathPos.y);
            float3 moveDir = math.normalizesafe(targetPos - localTransform.ValueRW.Position);
            float speed = 1f;

            localTransform.ValueRW.Position += moveDir * speed * deltaTime;
            
            //Determine if we're at our node
            if(math.distance(localTransform.ValueRW.Position, targetPos) < 0.1f)
            {
                pathIndex.ValueRW.pathIndex -= 1;
                if(pathIndex.ValueRW.pathIndex == -1)
                {
                    pp.ValueRW.startPos = pathPos[0].position;
                }
            }
        }
    }

    [BurstCompile]
    private partial struct RandomNodeJob : IJobEntity
    {
        [NativeDisableUnsafePtrRestriction]
        public RefRW<RandomComponent> random;

        [BurstCompile]
        public void Execute(RefRW<PathfindingParams> pp, RefRW<PathIndexComponent> pathIndex)
        {
            if(pathIndex.ValueRW.pathIndex > -1)
            {
                return;
            }
            pp.ValueRW.endPos = new int2(random.ValueRW.random.NextInt(0, 20), random.ValueRW.random.NextInt(0, 20));
        }
    }
} 