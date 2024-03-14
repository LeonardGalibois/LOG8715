using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using System.Collections;
using Unity.Collections;

[BurstCompile]
public partial struct ClosestPreySystem : Unity.Entities.ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PreyComp>();
        state.RequireForUpdate<ClosestPreyComp>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var preysPosition = SystemAPI.QueryBuilder().WithAll<PreyComp, LocalTransform>().Build().ToComponentDataArray<LocalTransform>(Allocator.TempJob);

        var job = new ClosestPreyJob { preys = preysPosition };
        var handle = job.ScheduleParallel(state.Dependency);

        handle.Complete();
        preysPosition.Dispose();
    }
}

[BurstCompile]
public partial struct ClosestPreyJob : IJobEntity
{
    public NativeArray<LocalTransform> preys;

    [BurstCompile]
    public void Execute(in LocalTransform localTransform, ref ClosestPreyComp closestPlantComp)
    {
        float closestDistance = float.MaxValue;
        foreach (LocalTransform prey in preys)
        {
            float distance = math.distance(localTransform.Position, prey.Position);
            if (distance <= closestDistance)
            {
                closestPlantComp.position = prey.Position;
                closestDistance = distance;
            }
        }
    }
}
