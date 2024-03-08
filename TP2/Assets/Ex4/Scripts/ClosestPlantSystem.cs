using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using System.Collections;
using Unity.Collections;
public partial struct ClosestPlantSystem : Unity.Entities.ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlantComp>();
        state.RequireForUpdate<ClosestPlantComp>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var plantsPosition = SystemAPI.QueryBuilder().WithAll<PlantComp, LocalTransform>().Build().ToComponentDataArray<LocalTransform>(Allocator.TempJob);

        var job = new ClosestPlantJob { plants = plantsPosition };
        var handle = job.ScheduleParallel(state.Dependency);

        handle.Complete();
        plantsPosition.Dispose();
    }
}

[BurstCompile]
public partial struct ClosestPlantJob : IJobEntity
{
    public NativeArray<LocalTransform> plants;
    public void Execute(in LocalTransform localTransform, ref ClosestPlantComp closestPlantComp )
    {
        float closestDistance = float.MaxValue;
        foreach (LocalTransform plant in plants)
        {
            float distance = math.distance(localTransform.Position, plant.Position);
            if (distance <= closestDistance)
            {
                closestPlantComp.position = plant.Position;
                closestDistance = distance;
            }
        }
    }
}