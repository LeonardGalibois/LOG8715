using Unity.Entities;
using Unity.Mathematics;

public struct LifeTimeComp : IComponentData
{
    public float lifetime;
    public float startingLifetime;
    public float decreasingFactor;
}

public struct VelocityComp : IComponentData
{
    public float3 direction;
    public float speed;
}

public struct PlantComp : IComponentData { }
public struct PreyComp : IComponentData 
{
    public bool reproduce;
}
public struct ClosestPlantComp : IComponentData
{
    public float3 position;
}

public struct PredatorComp : IComponentData 
{
    public bool reproduce;
}
public struct ClosestPreyComp : IComponentData
{
    public float3 position;
}

public struct SpawnerComp : IComponentData
{
    public Entity plantPrefab;
    public Entity preyPrefab;
    public Entity predatorPrefab;

    public int plantCount;
    public int preyCount;
    public int predatorCount;
    public int gridSize;

    public int halfWidth;
    public int halfHeight;
}