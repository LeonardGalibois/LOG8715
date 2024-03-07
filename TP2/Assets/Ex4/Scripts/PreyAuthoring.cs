using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class PreyAuthoring : MonoBehaviour
{
    class Baker : Baker<PreyAuthoring>
    {
        public override void Bake(PreyAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);
            float startingLife = UnityEngine.Random.Range(Lifetime.StartingLifetimeLowerBound, Lifetime.StartingLifetimeUpperBound);

            AddComponent(entity, new PreyComp { });
            AddComponent(entity, new LifeTimeComp { 
                lifetime = startingLife,
                startingLifetime = startingLife,
                decreasingFactor = 1
            });
            AddComponent(entity, new VelocityComp
            {
                direction = float3.zero,
                speed = Ex4Config.PreySpeed
            });
        }
    }
}
