using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using System.Collections;
using Unity.Collections;
public partial struct ChangePredatorLifetimeSystem : Unity.Entities.ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ClosestPreyComp>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var predatorsPosition = SystemAPI.QueryBuilder().WithAll<PredatorComp, LocalTransform>().Build().ToComponentDataArray<LocalTransform>(Allocator.TempJob);

        var job = new ChangePredatorLifetimeJob
        {
            predators = predatorsPosition,
        };

        var handle = job.ScheduleParallel(state.Dependency);

        handle.Complete();
        predatorsPosition.Dispose();
    }
}
[BurstCompile]
public partial struct ChangePredatorLifetimeJob : IJobEntity
{
    public NativeArray<LocalTransform> predators;
    [BurstCompile]
    public void Execute(in LocalTransform localTransform, ref LifeTimeComp lifetimeComp, ref PredatorComp predatorComp, in ClosestPreyComp closestPreyComp)
    {
        lifetimeComp.decreasingFactor = 1f;
        if (math.distance(localTransform.Position, closestPreyComp.position) <= Ex4Config.TouchingDistance)
        {
            lifetimeComp.decreasingFactor /= 2f;
        }

        foreach (LocalTransform predator in predators)
        {
            float distance = math.distance(localTransform.Position, predator.Position);
            if (distance <= Ex4Config.TouchingDistance && math.EPSILON <= distance)
            {
                predatorComp.reproduce = true;
                break;
            }
        }
    }
}