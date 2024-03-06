using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Transforms;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct VelocitySystem : Unity.Entities.ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var job = new VelocityJob { deltaTime = SystemAPI.Time.DeltaTime };
        job.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct VelocityJob : IJobEntity
{
    public float deltaTime;

    public void Execute(ref LocalTransform transform, in VelocityComp velocity)
    {
        transform = transform.Translate(velocity.direction * velocity.speed * deltaTime);
    }
}
