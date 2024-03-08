using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using System.Collections;
using Unity.Collections;

[BurstCompile]
public partial struct MoveTowardPreySystem : Unity.Entities.ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ClosestPreyComp>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var job = new MoveTowardPreyJob { };
        job.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct MoveTowardPreyJob : IJobEntity
{
    [BurstCompile]
    public void Execute(ref VelocityComp velocityComp, in ClosestPreyComp closestPreyComp, in LocalTransform localTransform)
    {
        velocityComp.direction = math.normalize(closestPreyComp.position - localTransform.Position);
    }
}
