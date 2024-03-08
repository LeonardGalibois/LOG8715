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
            float startingLife = UnityEngine.Random.Range(LifetimeSystem.StartingLifetimeLowerBound, LifetimeSystem.StartingLifetimeUpperBound);

            AddComponent(entity, new PreyComp
            {
                reproduce = false
            });
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
            AddComponent(entity, new ClosestPlantComp
            {
                position = float3.zero
            });
        }
    }
}
