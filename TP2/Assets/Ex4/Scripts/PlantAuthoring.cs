using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class PlantAuthoring : MonoBehaviour
{
    class Baker : Baker<PlantAuthoring>
    {
        public override void Bake(PlantAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);
            float startingLife = UnityEngine.Random.Range(LifetimeSystem.StartingLifetimeLowerBound, LifetimeSystem.StartingLifetimeUpperBound);

            AddComponent(entity, new PlantComp { });
            AddComponent(entity, new LifeTimeComp
            {
                lifetime = startingLife,
                startingLifetime = startingLife,
                decreasingFactor = 1
            });
        }
    }
}
