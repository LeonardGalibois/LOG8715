using Unity.Transforms;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;

[BurstCompile]
public partial struct LifetimeSystem : Unity.Entities.ISystem
{
    public const float StartingLifetimeLowerBound = 5;
    public const float StartingLifetimeUpperBound = 15;

    uint updateCounter;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SpawnerComp>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        SpawnerComp spawnerComp = SystemAPI.GetSingleton<SpawnerComp>();
        Random randomGenerator = Random.CreateFromIndex(updateCounter++);

        var plantJob = new PlantLifetimeJob 
        { 
            deltaTime = SystemAPI.Time.DeltaTime,
            spawner = spawnerComp,
            random = randomGenerator
        };
        var preyJob = new PreyLifetimeJob 
        { 
            deltaTime = SystemAPI.Time.DeltaTime,
            spawner = spawnerComp,
            random = randomGenerator
        };
        var predatorJob = new PredatorLifetimeJob 
        { 
            deltaTime = SystemAPI.Time.DeltaTime,
            spawner = spawnerComp,
            random = randomGenerator,
        };
        
        plantJob.ScheduleParallel();
        preyJob.ScheduleParallel();
        predatorJob.ScheduleParallel();
    }
}
[BurstCompile]
public partial struct PlantLifetimeJob : IJobEntity
{
    public float deltaTime;
    public SpawnerComp spawner;
    public Random random;
    [BurstCompile]
    public void Execute(ref LifeTimeComp lifeTime, ref LocalTransform localTransform, in PlantComp plantComp)
    {
        lifeTime.lifetime -= lifeTime.decreasingFactor * deltaTime;
        localTransform.Scale = lifeTime.lifetime / lifeTime.startingLifetime;
        if (lifeTime.lifetime > 0) return;

        lifeTime.lifetime = lifeTime.startingLifetime;
        localTransform.Position = new int3(random.NextInt(-spawner.halfWidth, spawner.halfWidth), random.NextInt(-spawner.halfHeight, spawner.halfHeight), 0);
    }
}

[BurstCompile]
public partial struct PreyLifetimeJob : IJobEntity
{
    public float deltaTime;
    public SpawnerComp spawner;
    public Random random;
    [BurstCompile]
    public void Execute(ref LifeTimeComp lifeTime, ref LocalTransform localTransform, ref PreyComp preyComp)
    {
        lifeTime.lifetime -= lifeTime.decreasingFactor * deltaTime;
        if (lifeTime.lifetime > 0) return;

        if (preyComp.reproduce)
        {
            lifeTime.lifetime = lifeTime.startingLifetime;
            localTransform.Position = new int3(random.NextInt(-spawner.halfWidth, spawner.halfWidth), random.NextInt(-spawner.halfHeight, spawner.halfHeight), 0);
            preyComp.reproduce = false;
        }
    }
}

[BurstCompile]
public partial struct PredatorLifetimeJob : IJobEntity
{
    public float deltaTime;
    public SpawnerComp spawner;
    public Random random;
    public void Execute( ref LifeTimeComp lifeTime, ref LocalTransform localTransform, ref PredatorComp predatorComp)
    {
        lifeTime.lifetime -= lifeTime.decreasingFactor * deltaTime;
        if (lifeTime.lifetime > 0) return;

        if (predatorComp.reproduce)
        {
            lifeTime.lifetime = lifeTime.startingLifetime;
            localTransform.Position = new int3(random.NextInt(-spawner.halfWidth, spawner.halfWidth), random.NextInt(-spawner.halfHeight, spawner.halfHeight), 0);
            predatorComp.reproduce = false;
        }
    }
}
