using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using System.Collections;
using Unity.Collections;
public partial struct ChangePreyLifetimeSystem : Unity.Entities.ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ClosestPlantComp>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var preysPosition = SystemAPI.QueryBuilder().WithAll<PreyComp, LocalTransform>().Build().ToComponentDataArray<LocalTransform>(Allocator.TempJob);
        var predatorsPosition = SystemAPI.QueryBuilder().WithAll<PredatorComp, LocalTransform>().Build().ToComponentDataArray<LocalTransform>(Allocator.TempJob);

        var job = new ChangePreyLifetimeJob
        {
            predators = predatorsPosition,
            preys = preysPosition,
        };

        var handle = job.ScheduleParallel(state.Dependency);
        handle.Complete();

        predatorsPosition.Dispose();
        preysPosition.Dispose();
    }
}
[BurstCompile]
public partial struct ChangePreyLifetimeJob : IJobEntity
{
    public NativeArray<LocalTransform> predators;
    public NativeArray<LocalTransform> preys;
    [BurstCompile]
    public void Execute(in LocalTransform localTransform, ref LifeTimeComp lifetimeComp, ref PreyComp preyComp, in ClosestPlantComp closestPlantComp)
    {
        lifetimeComp.decreasingFactor = 1f;
        if(math.distance(localTransform.Position, closestPlantComp.position) <= Ex4Config.TouchingDistance)
        {
            lifetimeComp.decreasingFactor /= 2f;
        }

        foreach (LocalTransform predator in predators)
        {
            if (math.distance(localTransform.Position, predator.Position) <= Ex4Config.TouchingDistance)
            {
                lifetimeComp.decreasingFactor *= 2f;
                break;
            }
        }

        foreach (LocalTransform prey in preys)
        {
            float distance = math.distance(localTransform.Position, prey.Position);
            if (distance <= Ex4Config.TouchingDistance && math.EPSILON <= distance)
            {
                preyComp.reproduce = true;
                break;
            }
        }
    }
}
