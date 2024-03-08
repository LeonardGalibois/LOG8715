using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using System.Collections;
using Unity.Collections;

[BurstCompile]
public partial struct MoveTowardPlantSystem : Unity.Entities.ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ClosestPlantComp>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var job = new MoveTowardPlantJob { };
        job.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct MoveTowardPlantJob : IJobEntity
{
    [BurstCompile]
    public void Execute(ref VelocityComp velocityComp, in ClosestPlantComp closestPlantComp, in LocalTransform localTransform)
    {
        velocityComp.direction = math.normalize(closestPlantComp.position - localTransform.Position);
    }
}