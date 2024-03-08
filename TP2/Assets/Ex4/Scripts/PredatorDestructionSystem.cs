using Unity.Transforms;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public partial struct DestructionSystem : Unity.Entities.ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PredatorComp>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var commandBufferSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var commandBuffer = commandBufferSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach(var (lifetimeComp, predatorComp, entity) in SystemAPI.Query<RefRO<LifeTimeComp>, RefRO<PredatorComp>>().WithEntityAccess())
        {
            if(lifetimeComp.ValueRO.lifetime <= 0  && !predatorComp.ValueRO.reproduce)
            {
                commandBuffer.DestroyEntity(entity);
            }
        }

        foreach (var (lifetimeComp, preyComp, entity) in SystemAPI.Query<RefRO<LifeTimeComp>, RefRO<PreyComp>>().WithEntityAccess())
        {
            if (lifetimeComp.ValueRO.lifetime <= 0 && !preyComp.ValueRO.reproduce)
            {
                commandBuffer.DestroyEntity(entity);
            }
        }
    }
}
