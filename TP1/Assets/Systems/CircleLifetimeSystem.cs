using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Config;

namespace Assets.Systems
{
    public class CircleLifetimeSystem : ISystem
    {
        public CircleLifetimeSystem()
        {
            foreach (ShapeConfig shapeConfig in ECSController.Instance.Config.circleInstancesToSpawn)
            {
                uint entity = World.currentWorld.CreateEntity();

                World.currentWorld.AddComponent(entity, new PositionComponent(shapeConfig.initialPosition));
                World.currentWorld.AddComponent(entity, new SizeComponent(shapeConfig.initialSize));
                World.currentWorld.AddComponent(entity, new SpeedComponent(shapeConfig.initialVelocity));
                World.currentWorld.AddComponent(entity, new ColorComponent(Color.white));
                World.currentWorld.AddComponent(entity, new CollisionComponent(0));

                ECSController.Instance.CreateShape(entity, shapeConfig.initialSize);
            }
        }

        public string Name => "CircleLifetimeSystem";

        public void UpdateSystem()
        {
            var dictionary = World.currentWorld.GetAllComponents<SizeComponent>();

            if (dictionary is null) return;

            foreach (var item in dictionary)
            {
                SizeComponent sizeComponent = item.Value as SizeComponent;

                if (sizeComponent is not null && sizeComponent.size == 0)
                {
                    World.currentWorld.DeleteEntity(item.Key);
                    ECSController.Instance.DestroyShape(item.Key);
                }
            }
        }
    }
}
