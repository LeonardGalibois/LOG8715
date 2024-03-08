using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
public partial struct Ex4SpawnSystem : Unity.Entities.ISystem
{
    public bool spawnPrefab;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SpawnerComp>();
        spawnPrefab = true;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (spawnPrefab)
        {
            spawnPrefab = false;
            SpawnerComp spawner = SystemAPI.GetSingleton<SpawnerComp>();

            // Spawn the plants
            if (SystemAPI.QueryBuilder().WithAll<PlantComp>().Build().IsEmpty)
            {
                var plantInstances = state.EntityManager.Instantiate(spawner.plantPrefab, spawner.plantCount, Allocator.Temp);

                foreach (Entity plant in plantInstances)
                {
                    Respawn(ref state, plant, spawner);
                }
                plantInstances.Dispose();
            }

            // Spawn the prey
            if (SystemAPI.QueryBuilder().WithAll<PreyComp>().Build().IsEmpty)
            {
                var preysInstances = state.EntityManager.Instantiate(spawner.preyPrefab, spawner.preyCount, Allocator.Temp);

                foreach (Entity preys in preysInstances)
                {
                    Respawn(ref state, preys, spawner);
                }

                preysInstances.Dispose();
            }

            // Spawn the predator
            if (SystemAPI.QueryBuilder().WithAll<PredatorComp>().Build().IsEmpty)
            {
                var predatorsInstances = state.EntityManager.Instantiate(spawner.predatorPrefab, spawner.predatorCount, Allocator.Temp);

                foreach (Entity predators in predatorsInstances)
                {
                    Respawn(ref state, predators, spawner);
                }

                predatorsInstances.Dispose();
            }
        }
        
    }

    [BurstCompile]
    public void Respawn(ref SystemState state, Entity entity, SpawnerComp spawner)
    {
        var localTransform = SystemAPI.GetComponentRW<LocalTransform>(entity);
        localTransform.ValueRW.Position = new int3(UnityEngine.Random.Range(-spawner.halfWidth, spawner.halfWidth), UnityEngine.Random.Range(-spawner.halfHeight, spawner.halfHeight), 0);
    }
}
