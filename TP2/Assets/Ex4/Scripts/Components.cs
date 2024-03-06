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
public struct PreyComp : IComponentData { }
public struct PredatorComp : IComponentData 
{
    bool reproduce;
}