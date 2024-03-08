using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using System.Collections;
using Unity.Collections;

[BurstCompile]
public partial struct ChangePlantLifetimeSystem : Unity.Entities.ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlantComp>();
        state.RequireForUpdate<LifeTimeComp>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var preysPosition = SystemAPI.QueryBuilder().WithAll<PreyComp, LocalTransform>().Build().ToComponentDataArray<LocalTransform>(Allocator.TempJob);

        var job = new ChangePlantLifetimeJob { preys = preysPosition };
        var handle = job.ScheduleParallel(state.Dependency);

        handle.Complete();
        preysPosition.Dispose();
    }
}
[BurstCompile]
public partial struct ChangePlantLifetimeJob : IJobEntity
{
    public NativeArray<LocalTransform> preys;
    [BurstCompile]
    public void Execute(in LocalTransform localTransform, ref LifeTimeComp lifetimeComp, in PlantComp plantComp)
    {
        lifetimeComp.decreasingFactor = 1;
        foreach(LocalTransform prey in preys)
        {
            if(math.distance(localTransform.Position, prey.Position) <= Ex4Config.TouchingDistance)
            {
                lifetimeComp.decreasingFactor *= 2;
                break;
            }
        }
    }
}
