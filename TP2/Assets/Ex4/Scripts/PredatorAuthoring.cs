using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class PredatorAuthoring : MonoBehaviour
{
    class Baker : Baker<PredatorAuthoring>
    {
        public override void Bake(PredatorAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale);
            float startingLife = UnityEngine.Random.Range(Lifetime.StartingLifetimeLowerBound, Lifetime.StartingLifetimeUpperBound);

            AddComponent(entity, new PredatorComp
            {
                reproduce = false
            });
            AddComponent(entity, new LifeTimeComp
            {
                lifetime = startingLife,
                startingLifetime = startingLife,
                decreasingFactor = 1
            });
            AddComponent(entity, new VelocityComp
            {
                direction = float3.zero,
                speed = Ex4Config.PredatorSpeed
            });
        }
    }
}
